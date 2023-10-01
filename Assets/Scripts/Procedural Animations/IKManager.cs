using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKManager : MonoBehaviour
{
    [Header("Hand IK Targets")]
    public Transform rightHand;
    public Transform leftHand;
    public TwoBoneIKConstraint leftIK;
    public TwoBoneIKConstraint rightIK;

    public EnumManager.TagObject tagObject;
    Transform target;
    float direction;

    void Update()
    {
        CheckPos(); // Cek posisi tembok kanan kiri
    }

    void CheckPos()
    {
        if (target != null)
        {
            Vector3 head = target.position - transform.position;
            direction = AngleToDirection(transform.forward, head);
        }
        else
            ResetWeight();

    }

    float AngleToDirection(Vector3 forward, Vector3 targetDirection)
    {
        Vector3 prep = Vector3.Cross(forward, targetDirection);
        float dir = Vector3.Dot(prep, transform.up);

        if (dir > 0f)
        {
            RaycastPoint(transform.right);
            rightIK.weight += 2 * Time.deltaTime;
            leftIK.weight -= 2 * Time.deltaTime;
            return 1f;
        }
        else if (dir < 0f)
        {
            RaycastPoint(-transform.right);
            leftIK.weight += 2 * Time.deltaTime;
            rightIK.weight -= 2 * Time.deltaTime;
            return -1f;
        }
        else
        {
            ResetWeight();
            return 0f;
        }
    } 

    void RaycastPoint(Vector3 direction) // Kalo udah dapet posisi tembok, cari titik permukaannya pake raycast, tempel IK Tangan ke point raycast tadi
    {
        Ray ray = new Ray(transform.position, transform.TransformDirection(direction * 3));
        if (Physics.Raycast(transform.GetComponent<Collider>().bounds.center, direction, out RaycastHit hit, 1f))
        {
            leftHand.rotation = Quaternion.identity;
            leftHand.position = hit.point + new Vector3(0, .5f);

            rightHand.rotation = Quaternion.Euler(180, 360, 0);
            rightHand.position = hit.point + new Vector3(0, .5f);
        }
    }

    void ResetWeight() // Hapus IK nya kalo udah menjauh dari tembok
    {
        leftIK.weight -= 2 * Time.deltaTime;
        rightIK.weight -= 2 * Time.deltaTime;
    } 

    private void OnTriggerEnter(Collider other) // Deteksi collider yang masuk, jadikan variabel tembok
    {
        target = other.transform;
        if(other.tag.Equals(tagObject))
        {

        }
    }

    private void OnTriggerExit(Collider other) // Deteksi collider yang keluar, hapus variabel tembok
    {
        target = null;
    }
}
