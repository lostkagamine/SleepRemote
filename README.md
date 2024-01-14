# SleepRemote
Control Windows power state remotely, quickly.

## Usage
This service ***requires*** a [Tailscale](https://tailscale.com) network interface and IPv4 address to be present within the machine, for security purposes. If Tailscale is not installed, or disconnected, or the machine is not present on a tailnet, the service will not start.

While the service is running, a HTTP listener will be served at `http://TAILSCALE_IP:6789`. Make a POST request to the following endpoints to perform actions.

- `/sleep` - Puts the machine to sleep. This method is instant and has no delay or warning.
- `/shutdown` - Shuts down the machine after a 20-second wait, notifying whoever is using the computer at the time.
- `/restart` - Restarts the machine after a 20-second wait, notifying whoever is using the computer at the time.

## Installation
Publish it with `dotnet publish`, register it as a Windows service with `sc.exe create`, then mark Tailscale as a dependency with `sc.exe config Name depend=Tailscale`. Then, go into `services.msc`, find it, mark it as Automatic (Delayed Start) and start it.

## License
```
SleepRemote - control Windows power state remotely, quickly
Copyright (C) 2024 Homura Akemi (Nightshade System) <hi@sylvie.software>

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
```