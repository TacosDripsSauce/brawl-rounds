using HarmonyLib;
using UnityEngine;

namespace BrawlRounds.Runtime
{
    internal sealed class MovementController : MonoBehaviour
    {
        private Player player;
        private GeneralInput input;
        private PlayerVelocity velocity;
        private bool held;
        private float tapDeadline;
        private int taps;
        private float dashReadyAt;
        private int airDashes;
        private float lastDir;

        private void Start()
        {
            player=GetComponent<Player>(); input=GetComponent<GeneralInput>(); velocity=GetComponent<PlayerVelocity>();
        }
        private void Update()
        {
            if (player == null || input == null || velocity == null || player.data == null || !player.data.view.IsMine) return;
            if (player.data.isGrounded) airDashes=1;
            float x=input.direction.x;
            if (Mathf.Abs(x)<0.1f) { held=false; return; }
            if (held) return;
            held=true;
            float sign=Mathf.Sign(x);
            if (Time.time>tapDeadline || sign!=lastDir) taps=0;
            lastDir=sign; taps++; tapDeadline=Time.time+0.23f;
            if (taps>=2 && Time.time>=dashReadyAt && (player.data.isGrounded || airDashes>0))
            {
                taps=0; dashReadyAt=Time.time+0.34f;
                if (!player.data.isGrounded) airDashes--;
                float mass=1f;
                try { mass=Traverse.Create(velocity).Field("mass").GetValue<float>(); } catch { }
                try { Traverse.Create(velocity).Method("AddForce", new object[]{new Vector2(sign*34f*mass, 2.2f*mass)}).GetValue(); } catch { }
            }
        }
    }
}
