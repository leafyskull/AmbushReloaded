using UnityEngine;

public class WorldMapTile : GameTile
{
    public Squad OccupyingSquad { get; private set; }
    public Supplies Supplies { get; private set; }

    public bool IsOccupied => OccupyingSquad != null;
    private SpriteRenderer squadMarker;

    // A small copy of the tile sprite serves as a temporary squad marker.
    public void ShowSquadMarker(Color color)
    {
        if (squadMarker == null)
        {
            SpriteRenderer ground = GetComponent<SpriteRenderer>();
            if (ground == null || ground.sprite == null) return;

            GameObject marker = new GameObject("Squad Marker");
            marker.transform.SetParent(transform, false);
            marker.transform.localScale = Vector3.one * 0.45f;
            squadMarker = marker.AddComponent<SpriteRenderer>();
            squadMarker.sprite = ground.sprite;
            squadMarker.sharedMaterial = ground.sharedMaterial;
            squadMarker.sortingLayerID = ground.sortingLayerID;
            squadMarker.sortingOrder = ground.sortingOrder + 1;
        }

        squadMarker.color = color;
        squadMarker.gameObject.SetActive(true);
    }

    // Pass null to clear the tile when a squad leaves.
    public void SetOccupyingSquad(Squad newOccupyingSquad)
    {
        OccupyingSquad = newOccupyingSquad;
        if (newOccupyingSquad == null && squadMarker != null)
        {
            squadMarker.gameObject.SetActive(false);
        }
    }

    public void SetSupplies(Supplies newSupplies)
    {
        Supplies = newSupplies;
    }
}
