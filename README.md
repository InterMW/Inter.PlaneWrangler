# Inter.PlaneWrangler

This microservice is in charge of taking in plane frame messages and, upon 
command (the tick message), it combines the given moment's set of plane info
and supplys that via web api.

# Pillars

## Plane Ingest

This pillar handles data intake for the service, in the form of a [PlaneFrameMessage](Application/Models/PlaneFrameMessage.cs).

The data is stored on a per-node, per-antenna, per-timestamp basis in redis.


## Plane Compiler

This pillar handles the compilation of plane data, prompted by the [TickMessage](Application/Models/TickMessage.cs).  The final result of this compilation is stored on a per-timestamp basis in redis.

## Access

This pillar is a web api, there to expose the compiled plane information to interested parties.