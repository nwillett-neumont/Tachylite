## Tech Stack Changes

- XUnit over NUnit. 10/5/2026. I decided to move to XUnit for it's simplicity, lightweight nature, and more robust support for the modern Microsoft Testing Framework.

## Architecture Changes

- Using more common practices in structure. 10/6/2026. I am deciding to change the architecture to use a Tachylite.Core folder instead of a Tachylite.Services folder so that I can use it to store my Models and ViewModels in addition to Services. I am also going to be making the base Tachylite project Tachylite.App for convention's sake.

- Revert Tachylite.App. 10/8/2026. Using the Tachylite.App project name caused namespacing issues in the project, so I decided to revert to fix.

## Implementation Changes

- Hold multiple recent documents. 10/5/2026. I decided to make the application persist multiple recent chests, so that the user doesn't need to find their most recently opened chests. The application will persist the 10 most recent chests.

- Store backups as a setting. 10/19/2026. I decided to support backing up files in both the user configuration directory and within the chest itself. This will be configurable by the user with four possible settings. Backup in chest, backup in user configuration directory, backup in both, or backups off.

- Implement SVG image rendering. 11/5/2026. I decided to leverage the Svg.Controls.Skia.Avalonia package to render my PDFs and Icons as SVG images where possible. I will still have support for fetching Bitmaps in the PDF Service, but I will likely not be using it. It is possible to make using Bitmaps a setting, but that is a change to be decided on at a later date.
