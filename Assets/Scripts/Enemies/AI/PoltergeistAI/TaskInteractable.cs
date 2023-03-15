﻿using BehaviorTree;
using UnityEngine;

namespace AI.PoltergeistAI
{
    public class TaskInteractable : Node
    {
        private Transform _transform;
        private float _speed;
        private bool _canInteract;

        public TaskInteractable(Transform transform, float speed, bool canInteract)
        {
            _transform = transform;
            _speed = speed;
            _canInteract = canInteract;
        }

        public override NodeState Evaluate()
        {
            Transform targetObject = (Transform)GetData("object");

            if (_canInteract)
            {
                AudioManager.Instance.PlaySFXRandom("Ghost_PG_Interact", 0.8f, 1.2f);
                if (targetObject.GetComponent<Interuptor>() || targetObject.parent.GetComponent<Interuptor>())
                {
                    if (Vector3.Distance(_transform.position, targetObject.position) > 1f)
                    {
                        _transform.position = Vector3.MoveTowards(
                            _transform.position, targetObject.position, _speed * Time.deltaTime);
                        _transform.LookAt(targetObject.position);
                        Parent.Parent.SetData("isInteract", true);
                    }
                    else
                    {
                        if (targetObject.GetComponent<Interuptor>())
                            targetObject.GetComponent<Interuptor>().Interact(new PlayerInteract());
                        if (targetObject.parent.GetComponent<Interuptor>())
                            targetObject.parent.GetComponent<Interuptor>().Interact(new PlayerInteract());
                        ClearData("object");
                        ClearData("isInteract");
                        _state = NodeState.FAILURE;
                        return _state;
                    }
                }
                else if (targetObject.GetComponent<Door>() || targetObject.parent.GetComponent<Door>())
                {
                    if (Vector3.Distance(_transform.position, targetObject.position) > 1f)
                    {
                        _transform.position = Vector3.MoveTowards(
                            _transform.position, targetObject.position, _speed * Time.deltaTime);
                        _transform.LookAt(targetObject.position);
                        Parent.Parent.SetData("isInteract", true);
                    }
                    else
                    {
                        if (targetObject.GetComponent<Door>()) targetObject.GetComponent<Door>().ToggleDoor();
                        if (targetObject.parent.GetComponent<Door>())
                            targetObject.parent.GetComponent<Door>().ToggleDoor();
                        ClearData("object");
                        ClearData("isInteract");
                        _state = NodeState.FAILURE;
                        return _state;
                    }
                }
                else
                {
                    if (targetObject.GetComponent<Animation>() || targetObject.parent.GetComponent<Animation>())
                    {
                        if (targetObject.GetComponent<Animation>()) targetObject.GetComponent<Animation>().Play();
                        if (targetObject.parent.GetComponent<Animation>())
                            targetObject.parent.GetComponent<Animation>().Play();
                    }
                    else
                    {
                        // TODO: Instantiate ParticleSystem on object
                        // Pooler.instance.Pop();
                        Debug.Log("Pop ParticleSystem");
                    }
                }

                ClearData("object");
                _state = NodeState.SUCCESS;
                return _state;
            }

            if (targetObject.GetComponent<Animation>())
            {
                targetObject.GetComponent<Animation>().Play();
            }
            else
            {
                // TODO: Instantiate ParticleSystem on object
                // Pooler.instance.Pop();
                Debug.Log("Pop ParticleSystem");
            }

            ClearData("object");
            _state = NodeState.SUCCESS;
            return _state;
        }
    }
}