using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class GeometryDashAgent : Agent
{
    public Rigidbody2D rb;
    public Transform obstacle;
    public LevelManager levelManager; // Quản lý các level
    private float jumpForce = 5f;

    public override void OnEpisodeBegin()
    {
        // Reset vị trí của agent và obstacle
        rb.velocity = Vector2.zero;
        rb.transform.position = new Vector2(-5, 0);
        levelManager.ResetLevel(); // Đặt lại level hiện tại
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Thu thập quan sát: khoảng cách tới chướng ngại vật, tốc độ, độ cao
        sensor.AddObservation(rb.transform.position.x);
        sensor.AddObservation(rb.velocity.y);
        sensor.AddObservation(obstacle.position.x - rb.transform.position.x);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Nhận hành động từ mô hình (0: không nhảy, 1: nhảy)
        if (actions.DiscreteActions[0] == 1 && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Phần thưởng: sống sót lâu hơn hoặc vượt qua chướng ngại vật
        AddReward(0.01f);

        // Phạt: chạm chướng ngại vật hoặc rơi
        if (rb.transform.position.y < -5 || Mathf.Abs(obstacle.position.x - rb.transform.position.x) < 0.5f)
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Mô phỏng hành động nhấn phím space từ người chơi
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}