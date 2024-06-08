using UnityEngine;

namespace Hearing
{
    public static class Sounds 
    {
        public static void MakeSound(Sound sound, LayerMask mask) 
        {
            Collider[] col = Physics.OverlapSphere(sound.pos, sound.range, mask);

            for (int i = 0; i < col.Length; i++)
            {
                if (col[i].TryGetComponent(out IHear hearer))
                {
                    hearer.RespondToSound(sound);
                }
            }
        }
    }
}