using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    // Configure services to be added to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews(); // Adds support for controllers and views
    }

    // Configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage(); // Provides detailed error pages in development
        }
        else
        {
            app.UseExceptionHandler("/Home/Error"); // Handles exceptions in production
            app.UseHsts(); // Adds HTTP Strict Transport Security
        }

        app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS
        app.UseStaticFiles(); // Enables serving static files

        app.UseRouting(); // Adds routing capabilities

        app.UseAuthorization(); // Adds authorization middleware

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute(); // Maps the default controller route
            endpoints.MapGet("/", async context => // Serve index.html at root
            {
                context.Response.ContentType = "text/html";
                using (var stream = File.OpenRead(Path.Combine(env.ContentRootPath, "index.html")))
                {
                    await stream.CopyToAsync(context.Response.Body);
                }
            });
            endpoints.MapGet("/index.html", async context => // Serve index.html
            {
                context.Response.ContentType = "text/html";
                using (var stream = File.OpenRead(Path.Combine(env.ContentRootPath, "index.html")))
                {
                    await stream.CopyToAsync(context.Response.Body);
                }
            });
            endpoints.MapGet("/fileEvents.json", async context => // Serve fileEvents.json
            {
                context.Response.ContentType = "application/json";
                using (var stream = File.OpenRead(Path.Combine(env.ContentRootPath, "fileEvents.json")))
                {
                    await stream.CopyToAsync(context.Response.Body);
                }
            });
        });
    }
}


class Program
{
    static void Main(string[] args)
    {
        string path = null;
        string userIdentifier = null;
        bool listUsers = false;
        bool trackFileExecutions = false; // New argument to track file executions
        bool serve = false; // New argument to serve index.html

        // Parse command line arguments
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--path" || args[i] == "-p")
            {
                if (i + 1 < args.Length)
                {
                    path = args[i + 1];
                }
            }
            else if (args[i] == "--serve")
            {
                serve = true;
            }
        }

        if (serve)
        {
            CreateHostBuilder(args).Build().Run();
            return;
        }

        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            Console.WriteLine("Please provide a valid directory path using --path (-p).");
            return;
        }

        // Start recursive file processing with hyperthreading
        var fileEvents = ProcessFilesRecursively(path).Result;

        // Output the file events to JSON file
        string jsonOutput = JsonConvert.SerializeObject(fileEvents, Formatting.Indented);
        string outputFilePath = Path.Combine(path, "fileEvents.json");
        File.WriteAllText(outputFilePath, jsonOutput);
        Console.WriteLine($"File events written to {outputFilePath}");
    }

    // Corrected the IHostBuilder method to use ConfigureWebHostDefaults
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://localhost:9999");
            });

    private static async Task<List<FileEvent>> ProcessFilesRecursively(string path)
    {
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);
        var fileEvents = new List<FileEvent>();

        // Process files in parallel
        var fileTasks = files.Select(file => Task.Run(() => 
        {
            var fileEvent = ProcessFile(file);
            lock (fileEvents) // Ensure thread safety when adding to the list
            {
                fileEvents.Add(fileEvent);
            }
        }));
        await Task.WhenAll(fileTasks);

        // Process directories recursively
        var directoryTasks = directories.Select(directory => ProcessFilesRecursively(directory));
        var directoryResults = await Task.WhenAll(directoryTasks);
        foreach (var result in directoryResults)
        {
            fileEvents.AddRange(result);
        }

        return fileEvents;
    }

    private static FileEvent ProcessFile(string file)
    {
        var fileInfo = new FileInfo(file);
        var fileEvent = new FileEvent
        {
            Name = fileInfo.Name,
            Path = fileInfo.FullName,
            Size = fileInfo.Length,
            Type = fileInfo.Extension,
            CreationDate = fileInfo.CreationTime,
            LastModifiedDate = fileInfo.LastWriteTime,
            LastAccessedDate = fileInfo.LastAccessTime,
            Owner = GetFileOwner(fileInfo),
            Events = new List<string>
            {
                $"File processed at {DateTime.Now}",
                $"Owner retrieved: {GetFileOwner(fileInfo)}"
            }
        };
        return fileEvent;
    }

    private static string GetFileOwner(FileInfo fileInfo)
    {
        var security = fileInfo.GetAccessControl();
        var owner = security.GetOwner(typeof(NTAccount));
        return owner.ToString();
    }


    public class FileEvent
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime LastAccessedDate { get; set; }
        public string Owner { get; set; }
        public List<string> Events { get; set; }
    }

    public class ExecutionEvent
    {
        public DateTime Time { get; set; }
        public string FileName { get; set; }
        public string ExecutingUser { get; set; }
        public string CommandLine { get; set; }
        public string ProcessId { get; set; }
        public string ParentProcessId { get; set; }
    }
}
