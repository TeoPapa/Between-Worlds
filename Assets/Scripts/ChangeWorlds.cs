using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChangeWorlds : MonoBehaviour
{
    public int SceneToChange;
    public Vector3 PositionToChange;
    public Vector3 RotationToChange;
    public bool ToFantasy;

    private void OnTriggerEnter(Collider other) {
        GameManager.Instance.ChangeScene(SceneToChange, ToFantasy, PositionToChange, RotationToChange);
    }
}
