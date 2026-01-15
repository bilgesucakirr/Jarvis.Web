# Jarvis.Web (Frontend)

This is the Blazor Server frontend application for the Academic Management System. It communicates with backend microservices (Identity, Submission, Review, Venue) to provide the user interface.

## Key Features

### 👤 Profile Management
*   **Profile Picture:** Users can upload and update their avatars.
*   **Extended Info:** Added fields for Affiliation, Country, Title, and Biography.
*   **Interest Selector:** Users choose research interests from a standardized, admin-managed list (Search & Chip UI).
*   **Security:** Password change functionality is integrated.

### 📊 Dashboard
*   **Search Bar:** Filters the "Available Venues" list in real-time by name or acronym.
*   **My Conferences:** Authors can view a dynamic list of their active submissions and status.
*   **Editor Tools:** Clear filters and CSV Export button for submission management.

### 📝 Submission & Review
*   **Camera Ready:** Workflow added for authors to upload the final version after acceptance.
*   **File Restrictions:** Enforces `.docx` format for manuscripts and review forms.
*   **Review Decision:** Added "Minor Revision" and "Major Revision" options.
*   **Blind Review:** Authors can view reviewer comments anonymously.

## Prerequisites

*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   SQL Server (LocalDB or Express)
*   **Backend Services must be running:**
    *   `Identity.Api` (Port: `7041`)
    *   `Submission.Api` (Port: `7004`)
    *   `Review.Api` (Port: `7047`)
    *   `Venue.Api` (Port: `7190`)

## How to Run

1.  Navigate to the project directory:
    ```bash
    cd src/Jarvis.Web
    ```

2.  Run the application:
    ```bash
    dotnet run
    ```

3.  Open your browser and go to:
    > https://localhost:7018

## Configuration

Backend API URLs are configured in `appsettings.json`. If your ports differ, update them here:

```json
{
  "IdentityApiUrl": "https://localhost:7041",
  "ApiBaseUrl": "https://localhost:7047",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
