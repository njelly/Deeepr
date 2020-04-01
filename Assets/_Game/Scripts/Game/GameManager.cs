using System.Collections.Generic;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        private List<Actor> _actors;
        private List<Floor> _floors;
        private Actor _playerActor;
        private int _currentLevel;

        private void Start()
        {
            _actors = new List<Actor>();
            _floors = new List<Floor>();

            _floors.Add(Floor.TestFloor());

            EnterLevel(0);
        }

        private void EnterLevel(int level)
        {
            _currentLevel = level;

            for(int i = 0; i < _floors.Count; i++)
            {
                if(i == _currentLevel)
                {
                    if(!_floors[i].IsBuilt)
                    {
                        _floors[i].Render(transform);
                    }
                }
                else
                {
                    if (_floors[i].IsBuilt)
                    {
                        _floors[i].GameObject.SetActive(false);
                    }
                }
            }
        }

        private void Update()
        {
            
        }
    }
}