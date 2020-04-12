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

        private const int FloorMinWidth = 50;
        private const int FloorMaxWidth = 80;
        private const int FloorMinHeight = 50;
        private const int FloorMaxHeight = 80;
        private const int RoomMinBufferSize = 6;
        private const int RoomMaxBufferSize = 30;

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

        private struct RoomGenData
        {
            public IntVector2Rect rect;
            public IntVector2[] doorTiles;
        }
        private static Floor DefaultGenAlgorithm(Random random, int level, IntVector2 floorSize)
        {
            List<IntVector2Rect> rooms = new List<IntVector2Rect>();

            // Step 1: determine the tiles types
            int[,] tileGenTypes = new int[floorSize.x, floorSize.y];

            IntVector2Rect CreateRoom()
            {
                int width = random.Next(RoomMinBufferSize, RoomMaxBufferSize);
                int height = random.Next(RoomMinBufferSize, RoomMaxBufferSize);

                IntVector2 min = new IntVector2(random.Next(1, floorSize.x - width - 1), random.Next(1, floorSize.y - height - 1));
                IntVector2 max = min + new IntVector2(width, height);

                return new IntVector2Rect(min, max);
            }

            // randomly try to populate the floor with non-overlapping
            for (int i = 0; i < 1000; i++)
            {
                IntVector2Rect room = CreateRoom();

                bool doesOverlap = false;
                foreach(IntVector2Rect existingRoom in rooms)
                {
                    if(existingRoom.Overlaps(room))
                    {
                        doesOverlap = true;
                        break;
                    }
                }

                if(doesOverlap)
                {
                    continue;
                }

                rooms.Add(room);
            }

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
                int numDoors = random.Next(2, 4); // 2, or 3 doors
                List<IntVector2> doorTiles = new List<IntVector2>();
                for (int i = 0; i < numDoors && potentialDoorCoords.Count > 0; i++)
                {
                    int index = random.Next(0, potentialDoorCoords.Count);
                    IntVector2 doorCoord = potentialDoorCoords[index];

                    // remove nearby potential door coords
                    List<int> toRemove = new List<int>();
                    for(int j = 0; j < potentialDoorCoords.Count; j++)
                    {
                        if((potentialDoorCoords[j] - doorCoord).ManhattanDistance <= 3)
                        {
                            toRemove.Add(j);
                        }
                    }
                    List<IntVector2> remainingDoorCoords = new List<IntVector2>();
                    for(int k = 0; k < potentialDoorCoords.Count; k++)
                    {
                        if(!toRemove.Contains(k))
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

            // now place the LadderDown tile
            List<IntVector2> potentialLadderCoords = new List<IntVector2>();
            for (int x = 0; x < tileGenTypes.GetLength(0); x++)
            {
                for (int y = 0; y < tileGenTypes.GetLength(1); y++)
                {
                    ETileGenType type = (ETileGenType)tileGenTypes[x, y];
                    if(type != ETileGenType.RoomFloor)
                    {
                        continue;
                    }

                    potentialLadderCoords.Add(new IntVector2(x, y));
                }
            }
            IntVector2 ladderUpCoord = potentialLadderCoords[random.Next(0, potentialLadderCoords.Count)];
            tileGenTypes[ladderUpCoord.x, ladderUpCoord.y] = (int)ETileGenType.LadderUp;

            // now place the LadderUp tile
            potentialLadderCoords.RemoveAll((IntVector2 x) => { return (x - ladderUpCoord).ManhattanDistance < 3; });
            IntVector2 ladderDownCoord = potentialLadderCoords[random.Next(0, potentialLadderCoords.Count)];

            tileGenTypes[ladderDownCoord.x, ladderDownCoord.y] = (int)ETileGenType.LadderDown;

            // build all the cooridors
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

                    ETileGenType type = (ETileGenType)tileGenTypes[coord.x, coord.y];
                    if (type == ETileGenType.RoomWall
                        || type == ETileGenType.RoomFloor
                        || type == ETileGenType.LadderUp
                        || type == ETileGenType.LadderUp)
                    {
                        return false;
                    }

                    return true;
                }, 9999);

                void TryMarkAsCorridorFloor(IntVector2 coord)
                {
                    ETileGenType currentTile = (ETileGenType)tileGenTypes[coord.x, coord.y];
                    if (currentTile == ETileGenType.Void || currentTile == ETileGenType.RoomWall || currentTile == ETileGenType.CorridorWall)
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
                    ETileGenType currentTile = (ETileGenType)tileGenTypes[coord.x, coord.y];
                    if (currentTile == ETileGenType.Void)
                    {
                        tileGenTypes[coord.x, coord.y] = (int)ETileGenType.CorridorWall;
                    }
                }

                for (int k = 0; k < path.Length; k++)
                {
                    TryMarkAsCorridorFloor(path[k]);
                }
            }

            // TODO: remove invalid doors

            // convert from gen types to actual tile types
            for (int x = 0; x < tileGenTypes.GetLength(0); x++)
            {
                for (int y = 0; y < tileGenTypes.GetLength(1); y++)
                {
                    tileGenTypes[x, y] = (int)FromGenType((ETileGenType)tileGenTypes[x, y]);
                }
            }

            // Step 2: populate the tiles with items, actors, etc.
            Floor toReturn = new Floor(tileGenTypes, level);

            return toReturn;
        }

        private static Tile.EType FromGenType(ETileGenType genType)
        {
            switch (genType)
            {
                case ETileGenType.RoomWall:
                case ETileGenType.CorridorWall:
                    return Tile.EType.Wall;
                case ETileGenType.LadderUp:
                    return Tile.EType.LadderUp;
                case ETileGenType.LadderDown:
                    return Tile.EType.LadderDown;
                case ETileGenType.Door:
                    return Tile.EType.Door;
                default:
                    return Tile.EType.Empty;
            }
        }
    }
}