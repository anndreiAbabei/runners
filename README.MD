# Runners

`//todo`

This is a personal project that I use to manage my local GitHub self-hosted runners. 

It uses an internal/personal library (SlimeTools.Commander) that is not open-sourced _yet_.
While the features will work (_hopefully_), you won't be able to run it without changing the implementation for the CLI args parser.

Atm code is kinda hacky, but I needed something quick to manage my runners. 
Not all console outputs is properly formated, unit tests are missing, windows code not tested, some code lies through `Constants` class, and there are some `DEBUG` directives left in the code while I was testing.

**So really, don't use this!** 

More details to add!

## How does it work?

Once started the application will first create a SQLite Database file in an "appdata" folder, and a folder called runners.
The database will store all the runners created through the application (using `add`) and their state. Also on the same folder a new subfolder will be created (called "runners") will be created, there all every GitHub runner will be created. 

Creating a new runner (`add`) will create a new folder with the name of the runner in the "runners" folder.

The said folder is as following:
- Windows: `C:\ProgramData\runners_app`
- Linux: `/var/lib/runners_app`
- OSX: `/Library/Application Support/runners_app`
- FreeBSD: `/var/db/runners_app` (_not supported yet_)

In that folder you will have the db file (`data.db`) and the folder with the runners (`runners`).

To remove everything you can simply delete the `runners_app` folder – I don't advice doing that tho!

--- 

## Usage
> runners {command} [{args}...]

### List of supported commands:

#### help
Prints the help message, with all commands & flags/arguments

Usage:
 ```shell
 runners help
 ``` 

---
#### version
Prints the version of the app

Usage:
 ```shell
 runners version
 ``` 

---
#### list
List all runners configured.
 
Usage:
 ```shell
 runners list
 ``` 

--- 
#### add
Create a new GitHub runner

- `--url {gitHubUrl}` Url of the GitHub url
- `--token {gitHubToken}` Token of the GitHub runner
- `--tag {tag}` Local name of the GitHub runner _[optional]_
- `--no-download`  Flag to stop add before download (default: `false`) _[optional]_
- `--no-config`  Flag to stop add before config (default: `false`) _[optional]_
- `--no-install`  Flag to stop add before installing (default: `false`) _[optional]_
- `--no-start`  Flag to stop add before starting (default: `false`) _[optional]_


Usage:
```shell
runners add 
    --url {gitHubUrl}
    --token {gitHubToken}
    [--tag {optionalTag}] 
    [--no-download] 
    [--no-config] 
    [--no-install] 
    [--no-start]
```

--- 
#### set
Change the state of a runner.
- `{RunnerId}`  Id of the runner that you want to set the state.
- `{start|stop}`  State of the runner start or stop.

Usage:
```shell
runners set {RunnerId} {start|stop}
```

--- 
#### delete
Change the state of a runner.
- `{RunnerId}`  Id of the runner that you want to set the state.
- `--yes` Required to set delete a runner.

Usage:
```shell
runners delete {RunnerId} [--yes {value}]
```