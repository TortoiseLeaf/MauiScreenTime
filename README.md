# MauiScreenTime

A screen time tracker that calculates app usage minutes to CO2eq

To install and run the application

```git clone git@github.com:TortoiseLeaf/MauiScreenTime.git```

Open the project in Visual Studio, and from the toolbar select `Build` to build the project.


From the taskbar, select the `tools` tab and then select `Android > Android Device Manager` 

you may need to create an android virtual device (avd). When a device is created, select it from the device manager and hit `play`. Once the avd is running hit the green arrow on the task bar to run the application on your emulator.

----

# Other useful project links:

[User Profiles](https://app.moqups.com/mxkatojdcdR2cgtLBnbcDKCccrIOlzAz/view/page/ad64222d5)

[Jira](https://uhimoraydev.atlassian.net/jira/software/c/projects/MAUIS/boards/4)

Gracjans CO2 fact sourcing software:

- [CO2 fact sourcing repo](https://github.com/Ryboster/AI-nature-fact-sourcing-tool)
- [CO2 fact sourcing frontend](https://www.blazejowski.co.uk/collaborations/co2_fact_submissions)

----
# Onboarding
## Contributing to MauiScreenTime

Hi everyone, thanks for contributing. This will behave probably as a living document that gets added to by contributors as we go, so feel free to contribute with things you think are helpful for the onboarding process.

The following is a set of guidelines for contributing to the MauiScreenTime app usage tracker. These are mostly guidelines, not steadfast rules. Use your best judgment, and feel free to propose changes and updates to this document in a pull request.

#### Table Of Contents

[Code of Conduct](#code-of-conduct)

[FAQ](#FAQ)

[What should I know before I get started?](#what-should-i-know-before-i-get-started)

[Design Decisions](#design-decisions)

[How Can I Contribute?](#how-can-i-contribute)



## Code of Conduct

This project and everyone participating in it is governed by a basic code of conduct. By participating, you are expected to uphold this code. Issues or conflicts we'll endeavour to resolve by just being transparent with each other in an open forum, but beyond that if any real issue persists you can report unacceptable behaviour to 19015831@uhi.ac.uk

Collaborators pledge to participate in the project and maintain a harassment-free and respectful environment for everyone.
This includes being respectful to different viewpoints, there's a 100 ways to skin a cat and programming is ultimately about problem solving - there's rarely one way to do something. Communication should be prioritized and constructive criticism is welcomed!
Remember we're all learning, so everyone has something valuable to contribute.

This Code of Conduct is loosely adapted from the [Atom Code of conduct](https://github.com/atom/atom/blob/master/CODE_OF_CONDUCT.md) 
## FAQ

> **Note:** This can be built on as we go, common problems will probably reveal themselves early on

**Q:** what should I put in a PR description?
**A:** As a general rule of thumb, just a couple of sentences like this:
```
## what does the PR do?
(describe the changes)

## Expected benefit
(this does what)

## Test Criteria
(How was it tested)

```

## What should I know before I get started?

### .NET MAUI and MVVM architecture

.NET MAUI is a C# framework for X-platform mobile apps. You write one codebase and it can build to different platforms. When a class file is created, a corresponding .xaml file is created to handle the UI for that class.

Crash course recommended by Team member: https://youtube.com/playlist?list=PLdo4fOcmZ0oUBAdL2NwBpDs32zwGqb9DY&si=RSPJjNPwqG7fwzQV

MauiScreenTime will be built using MVVM architecture, where a UI file (a View) will use a separate file for Logic (a ViewModel) to communicate with data (a Model). Basically instead of having everything in one file, it's split into three layers. This will keep the code clean and easy to maintain.
[MVVM explanation](https://www.geeksforgeeks.org/android/mvvm-model-view-viewmodel-architecture-pattern-in-android/)


file structure of folders we'll be working from mostly
```
── ...
├── Data                        # Dir for Models
|	├── UserConsentDb.cs
├── Pages                       # Dir for Views
|	├── ConsentPage.xaml
├── ViewModels                  # Dir for ViewModels
│   ├── ConsentViewModel.cs              
│   └── ... 
|          
├── App.cs                      # Where App is initialized    
├── MauiProgram.cs              # Where to inject libraries/dependencies
│                    
└── root
```


### Design Decisions

When we make a significant decision in how we build the project and wherever design changes might occur, we will document it in the [app UML](https://app.moqups.com/D97l8oJK6qShGe2DNts1beDq3X2Ts90W/view/page/ab0c2effe). Everyone has view access, but to edit it directly you'll need invited and to probably set up an account. If you make a change outside of a team meeting, please just leave a note next to it so we can all stay up to date. 
If you have a questions about anything, just put it in Teams.

## How Can I Contribute?

### Git workflow:

We'll be working from the `dev` branch only. Only full releases will be pushed to `master` once release criteria is achieved, so just pull and branch from `dev` when working on a ticket. 

You can name the branch the ticket name of the task you will work on and add a small descriptor, so for example Mauis-13-nature-db.

Remember when opening a Pull Request (PR) to rebase `dev` into your branch before merging your PR. This will ensure you have the latest version of dev on your branch before merging.


Protections are in place for these branches so that builds must succeed before merging, you can see the [github workflow yaml](https://github.com/TortoiseLeaf/MauiScreenTime/blob/dev/.github/workflows/dotnet.yml) to have a look if you're interested. As tests are added to the project they will also run automatically as part of this process when you create a PR, and will need to pass before you can merge.

PRs will require a review from at least 1 other person, once they're approved you can merge and delete your feature branch.

### DoD

At the moment the Definiton of Done for jira tickets is if the Pull Request is reviewed and accepted, and the branch check builds successfully.
When these conditions are satisfied, close the ticket

