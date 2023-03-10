using UnityEngine;

namespace _YabuGames.Scripts.Interfaces
{
    public interface IInteractable
    {
        void Interact(GameObject obj);
        void SetZone(bool onRange,float delay);
        void Select();

    }
}