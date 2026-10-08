## Tech Stack Changes

- XUnit over NUnit. 10/5/2026. I decided to move to XUnit for it's simplicity, lightweight nature, and more robust support for the modern Microsoft Testing Framework.

## Architecture Changes

- Using more common practices in structure. 10/6/2026. I am deciding to change the architecture to use a Tachylite.Core folder instead of a Tachylite.Services folder so that I can use it to store my Models and ViewModels in addition to Services. I am also going to be making the base Tachylite project Tachylite.App for convention's sake.

- Revert Tachylite.App. 10/8/2026. Using the Tachylite.App project name caused namespacing issues in the project, so I decided to revert to fix.

## Implementation Changes

- Hold multiple recent documents. 10/5/2026. I decided to make the application persist multiple recent chests, so that the user doesn't need to find their most recently opened chests. The application will persist the 10 most recent chests.
