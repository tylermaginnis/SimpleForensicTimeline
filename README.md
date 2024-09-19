# Forensic Timeline

## Live Demo

[Click here to view the live demo](https://tylermaginnis.github.io/SimpleForensicTimeline/)

## Overview

The Forensic Timeline project is a web application designed to visualize file events over time. It processes file metadata and events, allowing users to see when files were created, modified, accessed, and processed. The application utilizes D3.js for data visualization and supports light and dark themes.

## Features

- **Interactive Timeline**: Visual representation of file events with tooltips for detailed information.
- **Theme Toggle**: Switch between light and dark modes for better accessibility.
- **Responsive Design**: The application is designed to work on various screen sizes.
- **Event Processing**: Parses file events from a JSON file and displays them in a timeline format.

## Technologies Used

- **Frontend**: HTML, CSS, JavaScript
- **Backend**: ASP.NET Core (C#)
- **Data Format**: JSON for file events

## Installation

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 7.0 or later)
- A modern web browser (Chrome, Firefox, Edge, etc.)

### Steps

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/ForensicTimeline.git
   cd ForensicTimeline
   ```

2. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the Project**:
   ```bash
   dotnet build
   ```

4. **Run the Application**:
   ```bash
   ./bin/Debug/net7.0/ForensicTimeline.exe --path "C:\\path\\to\\audit\\
   ```

5. **Serve the Application**:
   ```bash
   ./bin/Debug/net7.0/ForensicTimeline.exe --server
   ```

5. **Access the Application**: Open your web browser and navigate to `http://localhost:9999` (or the specified port).

## Usage

1. **File Events JSON**: The application expects a `fileEvents.json` file in the root directory. This file should contain an array of file event objects with the following structure:
   ```json
   [
       {
           "Name": "example.txt",
           "CreationDate": "2024-09-17T13:12:39.9538805-04:00",
           "LastModifiedDate": "2024-09-17T13:12:39.9548791-04:00",
           "LastAccessedDate": "2024-09-19T12:09:02.4804509-04:00",
           "Owner": "BUILTIN\\Administrators",
           "Events": [
               "File processed at 9/19/2024 12:09:18 PM",
               "Owner retrieved: BUILTIN\\Administrators"
           ]
       }
   ]
   ```

2. **Interacting with the Timeline**: Hover over the events on the timeline to see detailed information in a tooltip. Click on events to view more details in a modal.

3. **Theme Toggle**: Use the toggle switch in the top right corner to switch between light and dark themes.

## Contributing

Contributions are welcome! If you have suggestions for improvements or new features, please open an issue or submit a pull request.

### Steps to Contribute

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes and commit them.
4. Push your branch to your forked repository.
5. Open a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [D3.js](https://d3js.org/) for data visualization.
- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) for backend development.
- [Normalize.css](https://necolas.github.io/normalize.css/) for consistent styling across browsers.

## Contact

For any inquiries, please contact [Your Name](mailto:your.email@example.com).