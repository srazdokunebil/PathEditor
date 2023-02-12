# WRobot PathEditor plugin for Vanilla 1.12.1

PathEditor allows Vector3 path XML data to be modified real-time from within the 5875 game client.  Use this tool to create paths that can be imported into Quester/Grinder/Gatherer profiles.

## Dependencies

- WRobot client (https://wrobot.eu/files/file/2-wrobot-official/)
- World of Warcraft 1.12.1 (5875) client

## Installation

- Place PathEditor.dll in **[wrobot root]\Plugins**

## Configuration

- Create the folowing macros in-game:


 **Num** | **Macro** | **Function** 
---|---|---
 1 | `/pe insert` | Inserts a node between the closest two nodes of your path. 
 2 | `/pe reposition` | Move the closest node to your current position. 
 4 | `/pe add` | Add a node connected to the last node of your path. 
 5 | `/pe del` | Deletes the closest node to your position. 
 6 | `/pe new` | Initializes new path.  CAREFUL!  This will delete your current path. 
 
## Usage

Upon first run, PathEditor will look something like this:

![Imgur](https://i.imgur.com/FVhEFKo.png)<Br>

Use macro command `/pe new` to start a new path.  PathEditor will create the first node of the path, represented by a yellow circle on the UI overlay:

![Imgur](https://i.imgur.com/rF30M2P.png)<Br>

Move to a new location and use macro command `/pe add` to add a new node to the end of the path:

![Imgur](https://i.imgur.com/lIyo9LF.png)<Br>

Keep adding nodes with `/pe add` to flesh out your path:

![Imgur](https://i.imgur.com/oG59cEw.png)<Br>

Notice there are two lines extending to the closest two sequential nodes relative to your current position.
- Green line: closest node
- Purple line: farthest node

![Imgur](https://i.imgur.com/chj8OkZ.png)<Br>

Invoking macro command `/pe insert` will insert a new node sequentially between those two nodes:

![Imgur](https://i.imgur.com/H69SEHX.png)<Br>

The green line indicates which node is closest to you:

![Imgur](https://i.imgur.com/Qvtka57.png)<Br>

Invoking `/pe reposition` will move that node to your current position:

![Imgur](https://i.imgur.com/bZyXmDI.png)<Br>