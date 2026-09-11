# cs3500-fall21-tankproject
A public version of the Tank Project created for Software Practice 1 during the fall 2021 semester. Completed with partner programming alongside Abhiveer Sharma (https://www.linkedin.com/in/abhiveer-sharma-647479153/).

**WARNING**: Under University of Utah Policy 6-410 (https://regulations.utah.edu/academics/6-410.php), a student caught committing academic misconduct via plagiarism will be subject to discipline. If your class is participating in this project, do not copy this work.

## Overview
This project was originally designed to be a lesson on MVC application design concepts, client-server relationships, and basic networking concepts. Client applications can connect to a host server application to play a simple PvP tank game over a TCP connection on the local network. The client application runs on Windows forms while the server application is a simple CLI display.

## Running Locally
To run the project on a local machine:
1) Clone the repository onto your machine.
2) Open the solution in the TankWars directory with Visual Studio.
3) Install the required .NET Core 3.1 when prompted in the explorer.
4) Run the "View" project for an instance of the client, or the "Server" project for an instance of the server on localhost. There can be multiple clients, but you should only run one server.
5) A client has the following controls available:
- WASD to move the tank.
- Mouse to aim.
- Hold left click to fire.
- Press right click after acquiring a powerup to fire a powerful laser.
