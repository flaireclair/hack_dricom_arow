using UnityEngine;


public class ArowSampleControlCube : MonoBehaviour
{
    const float MOVE_SPEED = 80f;

    private void OnGUI()
    {
        GUI.Label(new Rect(400, 50, 400, 200), "矢印カーソルでキューブを移動。必要な方向のマップを自動ロード");
    }
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 velocity = new Vector3(h, 0, v);
        // キャラクターのローカル空間での方向に変換
        //        velocity = transform.TransformDirection(velocity);
        // キャラクターの移動
        transform.localPosition += velocity * MOVE_SPEED * Time.fixedDeltaTime;
    }
}
