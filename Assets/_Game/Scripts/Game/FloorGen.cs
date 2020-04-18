using System;
using System.Collections.Generic;
using System.Threading;
using Tofunaut.Animation;
using Tofunaut.Core;

namespace Tofunaut.Deeepr.Game
{
    public static class FloorGen
    {
        public enum ETileGenType
        {
            Void,
            RoomWall,
            CorridorWall,
            RoomFloor,
            CorridorFloor,
            Door,
            LadderUp,
            LadderDown,
        }

        private const int BedrockBuffer = 10;
        private const int FloorMinWidth = 50 + BedrockBuffer;
        private const int FloorMaxWidth = 80 + BedrockBuffer;
        private const int FloorMinHeight = 50 + BedrockBuffer;
        private const int FloorMaxHeight = 80 + BedrockBuffer;

        public delegate void ResultDelegate(Floor result);

        public static void GenerateFloor(int seed, int level, ResultDelegate onComplete)
        {
            object lockObj = new object();

            Floor floor = null;
            Thread genThread = new Thread(new ThreadStart(() =>
            {
                Floor floorObj = Generate();

                lock (lockObj)
                {
                    floor = floorObj;
                }
            }));

            Floor Generate()
            {
                Random random = new Random(seed + level);

                Floor f = DefaultGenAlgorithm(random, level, new IntVector2(random.Next(FloorMinWidth, FloorMaxWidth), random.Next(FloorMinHeight, FloorMaxHeight)));

                return f;
            }

            TofuAnimation waitForGeneration = new TofuAnimation()
                .Execute(() =>
                {
                    genThread.Start();
                })
                .WaitUntil(() =>
                {
                    lock (lockObj)
                    {
                        return floor != null;
                    }
                })
                .Then()
                .Execute(() =>
                {
                    onComplete(floor);
                })
                .Play();
        }

        private const int DefaultGen_RoomMinBufferSize = 8;
        private const int DefaultGen_RoomMaxBufferSize = 18;
        private const int DefaultGen_CooridorErosionSteps = 4;
        private struct RoomGenData
        {
            public IntVector2Rect rect;
            public IntVector2[] doorTiles;
        }
        private static Floor DefaultGenAlgorithm(Random random, int level, IntVector2 floorSize)
        {
            List<IntVector2Rect> rooms = new List<IntVector2Rect>();

            int[,] tileGenTypes = new int[floorSize.x, floorSize.y];

            IntVector2Rect CreateRoom()
            {
                int width = random.Next(DefaultGen_RoomMinBufferSize, DefaultGen_RoomMaxBufferSize);
                int height = random.Next(DefaultGen_RoomMinBufferSize, DefaultGen_RoomMaxBufferSize);

                IntVector2 min = new IntVector2(random.Next(BedrockBuffer, floorSize.x - width - BedrockBuffer), random.Next(BedrockBuffer, floorSize.y - height - BedrockBuffer));
                IntVector2 max = min + new IntVector2(width, height);

                return new IntVector2Rect(min, max);
            }

            // 1) randomly try to populate the floor with non-overlapping
            for (int i = 0; i < 1000; i++)
            {
                IntVector2Rect room = CreateRoom();

                bool doesOverlap = false;
                foreach (IntVector2Rect existingRoom in rooms)
                {
                    if (existingRoom.Overlaps(room))
                    {
                        doesOverlap = true;
                        break;
                    }
                }

                if (doesOverlap)
                {
                    continue;
                }

                rooms.Add(room);
            }

            // 2) place doors around rooms
            List<RoomGenData> roomGenDatas = new List<RoomGenData>();
            foreach (IntVector2Rect room in rooms)
            {
                RoomGenData roomGenData = new RoomGenData();
                roomGenData.rect = room;

                List<IntVector2> potentialDoorCoords = new List<IntVector2>();
                for (int x = 1; x <= room.Width - 1; x++)
                {
                    for (int y = 1; y <= room.Height - 1; y++)
                    {
                        int xCoord = x + room.min.x;
                        int yCoord = y + room.min.y;

                        int tileType;
                        if (x == 1 || y == 1 || x == room.Width - 1 || y == room.Height - 1)
                        {
                            tileType = (int)ETileGenType.RoomWall;

                            bool isCorner = (x == 1 && (y == 1 || y == room.Height - 1)) || (x == room.Width - 1 && (y == 1 || y == room.Height - 1));
                            if (!isCorner)
                            {
                                // make sure the door has room on the otherside for a corridor
                                bool interiorEnough = !(xCoord < 3 || xCoord > floorSize.x - 3 || yCoord < 3 || yCoord > floorSize.y - 3);
                                if (interiorEnough)
                                {
                                    potentialDoorCoords.Add(new IntVector2(xCoord, yCoord));
                                }
                            }
                        }
                        else
                        {
                            tileType = (int)ETileGenType.RoomFloor;
                        }

                        tileGenTypes[xCoord, yCoord] = tileType;
                    }
                }

                // place doors
                int numDoors = 2; // random.Next(2, 3); // 2, or 3 doors
                List<IntVector2> doorTiles = new List<IntVector2>();
                for (int i = 0; i < numDoors && potentialDoorCoords.Count > 0; i++)
                {
                    int index = random.Next(0, potentialDoorCoords.Count);
                    IntVector2 doorCoord = potentialDoorCoords[index];

                    // remove nearby potential door coords
                    List<int> toRemove = new List<int>();
                    for (int j = 0; j < potentialDoorCoords.Count; j++)
                    {
                        if ((potentialDoorCoords[j] - doorCoord).ManhattanDistance <= 3)
                        {
                            toRemove.Add(j);
                        }
                    }
                    List<IntVector2> remainingDoorCoords = new List<IntVector2>();
                    for (int k = 0; k < potentialDoorCoords.Count; k++)
                    {
                        if (!toRemove.Contains(k))
                        {
                            remainingDoorCoords.Add(potentialDoorCoords[k]);
                        }
                    }
                    potentialDoorCoords = new List<IntVector2>(remainingDoorCoords);

                    doorTiles.Add(doorCoord);
                    tileGenTypes[doorCoord.x, doorCoord.y] = (int)ETileGenType.Door;
                }

                roomGenData.doorTiles = doorTiles.ToArray();
                roomGenDatas.Add(roomGenData);
            }

            // 3) now place the LadderUp tile
            List<IntVector2> usefulCoords = new List<IntVector2>();
            for (int x = 0; x < tileGenTypes.GetLength(0); x++)
            {
                for (int y = 0; y < tileGenTypes.GetLength(1); y++)
                {
                    if (!IsCoordType(tileGenTypes, x, y, ETileGenType.RoomFloor))
                    {
                        continue;
                    }

                    usefulCoords.Add(new IntVector2(x, y));
                }
            }
            IntVector2 ladderUpCoord = usefulCoords[random.Next(0, usefulCoords.Count)];
            tileGenTypes[ladderUpCoord.x, ladderUpCoord.y] = (int)ETileGenType.LadderUp;

            // 4) now place the LadderDown tile
            usefulCoords.RemoveAll((IntVector2 x) => { return (x - ladderUpCoord).ManhattanDistance < 3; });
            IntVector2 ladderDownCoord = usefulCoords[random.Next(0, usefulCoords.Count)];

            tileGenTypes[ladderDownCoord.x, ladderDownCoord.y] = (int)ETileGenType.LadderDown;

            // 5) build all the cooridors
            usefulCoords.Clear(); // now use usefulCoords for cooridor tiles
            for (int i = 0; i < roomGenDatas.Count - 1; i++)
            {
                RoomGenData fromRoom = roomGenDatas[i];
                RoomGenData toRoom = roomGenDatas[i + 1];

                IntVector2 fromDoorTile = fromRoom.doorTiles[random.Next(0, fromRoom.doorTiles.Length)];
                IntVector2 toDoorTile = toRoom.doorTiles[random.Next(0, toRoom.doorTiles.Length)];

                IntVector2[] path = IntVector2PathFinder.GetPath(fromDoorTile, toDoorTile, (IntVector2 coord) =>
                {
                    // can travel delegate

                    if (coord.x < 1 || coord.x > tileGenTypes.GetLength(0) - 2 || coord.y < 1 || coord.y > tileGenTypes.GetLength(1) - 2)
                    {
                        // don't path along the perimeter of floor
                        return false;
                    }

                    if (IsCoordType(tileGenTypes, coord, ETileGenType.RoomWall, ETileGenType.RoomFloor, ETileGenType.LadderUp, ETileGenType.LadderUp))
                    {
                        return false;
                    }

                    return true;
                }, 9999);

                void TryMarkAsCorridorFloor(IntVector2 coord)
                {
                    ETileGenType currentTile = (ETileGenType)tileGenTypes[coord.x, coord.y];
                    if (IsCoordType(tileGenTypes, coord, ETileGenType.Void, ETileGenType.RoomWall, ETileGenType.CorridorWall))
                    {
                        tileGenTypes[coord.x, coord.y] = (int)ETileGenType.CorridorFloor;
                    }

                    TryMarkAsCorridorWall(coord + IntVector2.Up);
                    TryMarkAsCorridorWall(coord + IntVector2.Up + IntVector2.Left);
                    TryMarkAsCorridorWall(coord + IntVector2.Up + IntVector2.Right);
                    TryMarkAsCorridorWall(coord + IntVector2.Down);
                    TryMarkAsCorridorWall(coord + IntVector2.Down + IntVector2.Left);
                    TryMarkAsCorridorWall(coord + IntVector2.Down + IntVector2.Right);
                    TryMarkAsCorridorWall(coord + IntVector2.Left);
                    TryMarkAsCorridorWall(coord + IntVector2.Right);
                }

                void TryMarkAsCorridorWall(IntVector2 coord)
                {
                    if (IsCoordType(tileGenTypes, coord, ETileGenType.Void))
                    {
                        tileGenTypes[coord.x, coord.y] = (int)ETileGenType.CorridorWall;
                        usefulCoords.Add(coord);
                    }
                }

                for (int k = 0; k < path.Length; k++)
                {
                    TryMarkAsCorridorFloor(path[k]);
                }
            }

            // 5) erode cooridor walls to provide more walking space
            List<IntVector2> wallsToRemove = new List<IntVector2>();
            for (int i = 0; i < DefaultGen_CooridorErosionSteps; i++)
            {
                // remove corridor walls when adjacent to 2 non-collinear cooridor walls, and mark them as corridor floors
                foreach(IntVector2 coord in usefulCoords)
                {
                    IntVector2 up = coord + IntVector2.Up;
                    IntVector2 down = coord + IntVector2.Down;
                    IntVector2 left = coord + IntVector2.Left;
                    IntVector2 right = coord + IntVector2.Right;

                    // never remove corridor walls that are ajacent to void tiles
                    if (IsCoordType(tileGenTypes, up, ETileGenType.Void))
                    {
                        continue;
                    }
                    if (IsCoordType(tileGenTypes, down, ETileGenType.Void))
                    {
                        continue;
                    }
                    if (IsCoordType(tileGenTypes, left, ETileGenType.Void))
                    {
                        continue;
                    }
                    if (IsCoordType(tileGenTypes, right, ETileGenType.Void))
                    {
                        continue;
                    }

                    bool upIsCorridorWall = IsCoordType(tileGenTypes, up, ETileGenType.CorridorWall);
                    bool downIsCorridorWall = IsCoordType(tileGenTypes, down, ETileGenType.CorridorWall);
                    bool leftIsCorridorWall = IsCoordType(tileGenTypes, left, ETileGenType.CorridorWall);
                    bool rightIsCorridorWall = IsCoordType(tileGenTypes, right, ETileGenType.CorridorWall);

                    bool doErode = false;
                    doErode |= upIsCorridorWall && (rightIsCorridorWall || leftIsCorridorWall);
                    doErode |= downIsCorridorWall && (rightIsCorridorWall || leftIsCorridorWall);

                    if(!doErode)
                    {
                        int numSurroundingCorridorFloors = 0;
                        numSurroundingCorridorFloors += IsCoordType(tileGenTypes, up, ETileGenType.CorridorFloor) ? 1 : 0;
                        numSurroundingCorridorFloors += IsCoordType(tileGenTypes, down, ETileGenType.CorridorFloor) ? 1 : 0;
                        numSurroundingCorridorFloors += IsCoordType(tileGenTypes, left, ETileGenType.CorridorFloor) ? 1 : 0;
                        numSurroundingCorridorFloors += IsCoordType(tileGenTypes, right, ETileGenType.CorridorFloor) ? 1 : 0;

                        doErode |= numSurroundingCorridorFloors >= 3;
                    }

                    if(doErode)
                    {
                        tileGenTypes[coord.x, coord.y] = (int)ETileGenType.CorridorFloor;
                        wallsToRemove.Add(coord);
                    }
                }

                usefulCoords.RemoveAll((IntVector2 coord) => wallsToRemove.Contains(coord));
                wallsToRemove.Clear();
            }

            // 6) remove invalid doors
            foreach (RoomGenData roomGenData in roomGenDatas)
            {
                foreach (IntVector2 coord in roomGenData.doorTiles)
                {
                    IntVector2 up = coord + IntVector2.Up;
                    IntVector2 down = coord + IntVector2.Down;
                    IntVector2 left = coord + IntVector2.Left;
                    IntVector2 right = coord + IntVector2.Right;

                    // the door is valid if it opens to a corridor floor
                    if (IsCoordType(tileGenTypes, up, ETileGenType.CorridorFloor))
                    {
                        continue;
                    }
                    if (IsCoordType(tileGenTypes, down, ETileGenType.CorridorFloor))
                    {
                        continue;
                    }
                    if (IsCoordType(tileGenTypes, left, ETileGenType.CorridorFloor))
                    {
                        continue;
                    }
                    if (IsCoordType(tileGenTypes, right, ETileGenType.CorridorFloor))
                    {
                        continue;
                    }

                    tileGenTypes[coord.x, coord.y] = (int)ETileGenType.RoomWall;
                }
            }

            // 7 convert from gen types to actual tile types
            for (int x = 0; x < tileGenTypes.GetLength(0); x++)
            {
                for (int y = 0; y < tileGenTypes.GetLength(1); y++)
                {
                    tileGenTypes[x, y] = (int)FromGenType((ETileGenType)tileGenTypes[x, y]);
                }
            }

            Floor toReturn = new Floor(tileGenTypes, level);

            // todo: add spawns, random npcs, items, etc.

            return toReturn;
        }

        private static Tile.EType FromGenType(ETileGenType genType)
        {
            switch (genType)
            {
                case ETileGenType.Void:
                case ETileGenType.RoomWall:
                case ETileGenType.CorridorWall:
                    return Tile.EType.Wall;
                case ETileGenType.LadderUp:
                    return Tile.EType.LadderUp;
                case ETileGenType.LadderDown:
                    return Tile.EType.LadderDown;
                case ETileGenType.Door:
                    return Tile.EType.Door;
                case ETileGenType.CorridorFloor:
                case ETileGenType.RoomFloor:
                    return Tile.EType.Floor;
                default:
                    return Tile.EType.Empty;
            }
        }

        private static bool IsCoordType(int[,] tileGenTypes, IntVector2 coord, params ETileGenType[] types) => IsCoordType(tileGenTypes, coord.x, coord.y, types);
        private static bool IsCoordType(int[,] tileGenTypes, int x, int y, params ETileGenType[] types)
        {
            if (x < 0 || x >= tileGenTypes.GetLength(0) || y < 0 || y >= tileGenTypes.GetLength(1))
            {
                return false;
            }

            ETileGenType type = (ETileGenType)tileGenTypes[x, y];
            foreach(ETileGenType compareTo in types)
            {
                if(type == compareTo)
                {
                    return true;
                }
            }

            return false;
        }
    }
}