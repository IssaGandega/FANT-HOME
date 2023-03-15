using System.Collections.Generic;
using System.Linq;
using BehaviorTree;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class CheckPlayer : Node
    {
        private Transform _transform;
        private Animator _animator;
        private Ghost _ghostComponent;
        private List<GameObject> _players;

        public CheckPlayer(Transform transform)
        {
            _transform = transform;
            _animator = transform.GetComponent<Animator>();
            _ghostComponent = transform.GetComponent<Ghost>();
        }

        public override NodeState Evaluate()
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length < 1)
            {
                _state = NodeState.FAILURE;
                return _state;
            }
            _players = GameObject.FindGameObjectsWithTag("Player").ToList();
            Transform target;
            
            if (_ghostComponent.CurRoom == null)
            {
                float currentDistance;
                int currentIndex = 0;
                float previousDistance = 0;
                for (var index = 0; index < RoomsManager.Instance.rooms.Length; index++)
                {
                    var room = RoomsManager.Instance.rooms[index];
                    currentDistance = math.sqrt(math.lengthsq(_transform.position - room.Floors[0].transform.position));
                    if (currentDistance < previousDistance || previousDistance == 0)
                    {
                        previousDistance = currentDistance;
                        currentIndex = index;
                    }
                }
                _ghostComponent.CurRoom = RoomsManager.Instance.rooms[currentIndex];
            }

            if (_players.Count() < 1)
            {
                _state = NodeState.FAILURE;
                return _state;
            }
            
            object t = GetData("target");
            if (t == null)
            {
                _players.RemoveAll(player => player.GetComponent<PlayerHealth>().curHealth <= 0);

                if (_players.Count < 1)
                {
                    _state = NodeState.FAILURE;
                    return _state;
                }

                float currentDistance = math.sqrt(math.lengthsq(_transform.position - _players[0].transform.position));
                target = _players[0].transform;

                List<GameObject> playersInSameRoom = new();

                if (Physics.Raycast(_transform.position, -Vector3.up, out var hit, 10.0f))
                {
                    Debug.DrawRay(_transform.position, -Vector3.up * hit.distance, Color.red);
                    if (hit.transform.gameObject.CompareTag("Floor"))
                    {
                        foreach (var room in RoomsManager.Instance.rooms)
                        {
                            if (room.Floors.Contains(hit.transform.gameObject))
                            {
                                _ghostComponent.CurRoom = room;
                            }
                        }
                    }
                }

                foreach (var player in _players)
                {
                    if (player.GetComponent<Player>().curRoom == _ghostComponent.CurRoom)
                    {
                        playersInSameRoom.Add(player);
                    }

                    if (!(currentDistance > math.sqrt(math.lengthsq(_transform.position - player.transform.position))))
                        continue;
                    currentDistance = math.sqrt(math.lengthsq(_transform.position - player.transform.position));
                    target = player.transform;
                }

                if (playersInSameRoom.Count > 0)
                {
                    if (_players.Count == 1)
                    {
                        Parent.Parent.SetData("target", playersInSameRoom[0].transform);
                        _state = NodeState.SUCCESS;
                        return _state;
                    }

                    var random = Random.Range(0f, 1.01f);
                    if (random < 0.5f)
                    {
                        currentDistance =
                            math.sqrt(math.lengthsq(_transform.position - playersInSameRoom[0].transform.position));
                        target = playersInSameRoom[0].transform;

                        foreach (var player in playersInSameRoom)
                        {
                            if (!(currentDistance > math.sqrt(math.lengthsq(_transform.position - player.transform.position))))
                                continue;
                            currentDistance = math.sqrt(math.lengthsq(_transform.position - player.transform.position));
                            target = player.transform;
                        }
                        Parent.Parent.SetData("target", target);
                        _state = NodeState.SUCCESS;
                        return _state;
                    }
                    
                    var randomPlayer = Random.Range(0, _players.Count);
                    target = playersInSameRoom[randomPlayer].transform;
                    Parent.Parent.SetData("target", target);
                    _state = NodeState.SUCCESS;
                    return _state;
                }
                
                var randomChoice = Random.Range(0f, 1.01f);
                if (randomChoice < 0.5f)
                {
                    var randomPlayer = Random.Range(0, _players.Count);
                    target = _players[randomPlayer].transform;
                    Parent.Parent.SetData("target", target);
                    _state = NodeState.SUCCESS;
                    return _state;
                }

                Parent.Parent.SetData("target", target);
                _state = NodeState.SUCCESS;
                return _state;
            }

            target = (Transform)t;
            if (target.GetComponent<PlayerHealth>().curHealth <= 0)
            {
                ClearData("target");
                _state = NodeState.FAILURE;
                return _state;
            }

            if (target.GetComponent<Player>().curRoom != _ghostComponent.CurRoom)
            {
                List<GameObject> playersInSameRoom = new();

                if (Physics.Raycast(_transform.position, -Vector3.up, out var hit, 10.0f))
                {
                    Debug.DrawRay(_transform.position, -Vector3.up * hit.distance, Color.red);
                    if (hit.transform.gameObject.CompareTag("Floor"))
                    {
                        foreach (var room in RoomsManager.Instance.rooms)
                        {
                            if (room.Floors.Contains(hit.transform.gameObject))
                            {
                                _ghostComponent.CurRoom = room;
                            }
                        }
                    }
                }

                int currentIndex = 0;
                float previousDistance = 0;
                
                foreach (var player in _players)
                {
                    if (player.GetComponent<Player>().curRoom == _ghostComponent.CurRoom)
                    {
                        playersInSameRoom.Add(player);
                    }
                }

                if (playersInSameRoom.Count > 0)
                {
                    if (playersInSameRoom.Count == 1)
                    {
                        Parent.Parent.SetData("target", playersInSameRoom[0].transform);
                        _state = NodeState.SUCCESS;
                        return _state;
                    }

                    var random = Random.Range(0f, 1.01f);
                    if (random < 0.5f)
                    {
                        var randomPlayer = Random.Range(0, playersInSameRoom.Count());
                        Parent.Parent.SetData("target", playersInSameRoom[randomPlayer].transform);
                        _state = NodeState.SUCCESS;
                        return _state;
                    }

                    for (var index = 0; index < playersInSameRoom.Count; index++)
                    {
                        var player = playersInSameRoom[index];
                        float currentDistance = math.sqrt(math.lengthsq(_transform.position - hit.transform.position));
                        if (currentDistance < previousDistance || previousDistance == 0)
                        {
                            previousDistance = currentDistance;
                            currentIndex = index;
                        }
                    }

                    Parent.Parent.SetData("target", playersInSameRoom[currentIndex].transform);
                    _state = NodeState.SUCCESS;
                    return _state;
                }
                else
                {
                    float currentDistance = math.sqrt(math.lengthsq(_transform.position - _players[0].transform.position));
                    target = _players[0].transform;
                    foreach (var player in _players)
                    {
                        if (!(currentDistance > math.sqrt(math.lengthsq(_transform.position - player.transform.position))))
                            continue;
                        currentDistance = math.sqrt(math.lengthsq(_transform.position - player.transform.position));
                        target = player.transform;
                    }
                    
                    var random = Random.Range(0f, 1.01f);
                    if (random < 0.5f)
                    {
                        var randomPlayer = Random.Range(0, _players.Count());
                        Parent.Parent.SetData("target", _players[randomPlayer].transform);
                        _state = NodeState.SUCCESS;
                        return _state;
                    }

                    Parent.Parent.SetData("target", target);
                    _state = NodeState.SUCCESS;
                    return _state;
                }
            }
            
            _state = NodeState.SUCCESS;
            return _state;
        }
    }
}