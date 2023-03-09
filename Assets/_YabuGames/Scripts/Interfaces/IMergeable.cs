using _YabuGames.Scripts.Controllers;

namespace _YabuGames.Scripts.Interfaces
{
    public interface IMergeable
    {
        void Merge(RadioController radio);
        void Disappear();
        void HoldingEffect();
        void ReleaseEffect();

    }
}