using Fusion;
using UnityEngine;


public class PlayerInteractions : NetworkBehaviour
{
    private MeshRenderer _meshRenderer;
    [SerializeField] private NetworkObject _minion;

    [Networked, OnChangedRender(nameof(ColorChanged))]
    public Color NetworkedColor { get; set; }

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (HasInputAuthority == false)
            return;
        if (Input.GetButtonDown("Fire1"))
        {
            NetworkedColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
        if (Input.GetButtonDown("Fire2"))
        {
                Runner.Spawn(_minion, transform.position + transform.forward * 2f, Quaternion.identity, Runner.LocalPlayer);
        }
    }

    private void ColorChanged()
    {
            _meshRenderer.material.color = NetworkedColor;
    }
}
