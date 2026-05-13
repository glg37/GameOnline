using Fusion;
using UnityEngine;

public class CamController : NetworkBehaviour
{
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
            return;

        gameObject.SetActive(false);
    }
}
