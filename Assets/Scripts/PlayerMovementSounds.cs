using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSounds : SoundfulObject {
    Terrain CurrentTerrain;
    Transform Player;
    public LayerMask Ground;

    bool OnTerrain = false;
    RaycastHit hit;

    public List<string> StoneNames;
    public List<string> GrassNames;
    public List<string> MudNames;
    public List<string> SandNames;
    public List<string> WoodNames;


    public List<AudioSource> Sources;

    public AudioClip[] GrassSteps;
    public AudioClip[] MudSteps;
    public AudioClip[] WoodSteps;
    public AudioClip[] SandSteps;
    public AudioClip[] StoneSteps;

    public AudioClip[] ClothesSounds;

    protected override List<AudioSource> SetSources() {
        return Sources;
    }

    void Start() {
        Player = GameManager.Instance.GetMovement().gameObject.transform;
        CurrentTerrain = Terrain.activeTerrain;
    }

    private void Update() {

        Ray ray = new Ray(Player.position + Vector3.up * 0.1f, Vector3.down);

        if (Physics.Raycast(ray, out hit, 1f, Ground)) {
            if (hit.collider.gameObject.tag.Equals("Terrain")) {
                OnTerrain = true;
                return;
            }

            OnTerrain = false;
        }
    }

    public void Step() {
        PlaySound(GetCurrentGround());
        ClothSound();
    }

    public void ClothSound() {
        PlaySound(ClothesSounds[UnityEngine.Random.Range(0, ClothesSounds.Length - 1)]);
    }

    public AudioClip GetCurrentGround() {
        string CurrentSurface = "Stone";

        if (OnTerrain)
            CurrentSurface = FindTerrainGround();
        else
            CurrentSurface = FindGround(hit.collider.GetComponent<Renderer>().material.name);

        switch (CurrentSurface) {
            case "Grass":
                return GrassSteps[UnityEngine.Random.Range(0, GrassSteps.Length - 1)];
            case "Mud":
                return MudSteps[UnityEngine.Random.Range(0, MudSteps.Length - 1)];
            case "Wood":
                return WoodSteps[UnityEngine.Random.Range(0, WoodSteps.Length - 1)];
            case "Sand":
                return SandSteps[UnityEngine.Random.Range(0, SandSteps.Length - 1)];
            default:
                return StoneSteps[UnityEngine.Random.Range(0, StoneSteps.Length - 1)];
        }
    }

    string FindGround(string SurfaceValue) {

        foreach(string gr in StoneNames) {
            if (SurfaceValue.Contains(gr, StringComparison.OrdinalIgnoreCase))
                return "Stone";
        }

        foreach (string gr in GrassNames) {
            if (SurfaceValue.Contains(gr, StringComparison.OrdinalIgnoreCase))
                return "Grass";
        }

        foreach (string gr in MudNames) {
            if (SurfaceValue.Contains(gr, StringComparison.OrdinalIgnoreCase))
                return "Mud";
        }

        foreach (string gr in WoodNames) {
            if (SurfaceValue.Contains(gr, StringComparison.OrdinalIgnoreCase))
                return "Wood";
        }

        foreach (string gr in SandNames) {
            if (SurfaceValue.Contains(gr, StringComparison.OrdinalIgnoreCase))
                return "Sand";
        }

        return "Stone";
    }

    string FindTerrainGround() {
        Vector3 worldPos = Player.position;
        Vector3 terrainLocalPos = worldPos - CurrentTerrain.transform.position;

        TerrainData terrainData = CurrentTerrain.terrainData;

        int mapX = Mathf.RoundToInt((terrainLocalPos.x / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = Mathf.RoundToInt((terrainLocalPos.z / terrainData.size.z) * terrainData.alphamapHeight);


        mapX = Mathf.Clamp(mapX, 0, terrainData.alphamapWidth - 1);
        mapZ = Mathf.Clamp(mapZ, 0, terrainData.alphamapHeight - 1);

        float[,,] alphamap = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        float maxWeight = 0f;
        int dominantLayerIndex = 0;

        int totalLayers = alphamap.GetLength(2);
        for (int i = 0; i < totalLayers; i++) {
            if (alphamap[0, 0, i] > maxWeight) {
                maxWeight = alphamap[0, 0, i];
                dominantLayerIndex = i;
            }
        }

        return FindGround(terrainData.terrainLayers[dominantLayerIndex].name);
    }
}
