using UnityEngine;

public class FollowSeeingEyeSigil : MonoBehaviour
{
    public Transform seeingEyeSigil;
    // Update is called once per frame
    void Update()
    {
        if (seeingEyeSigil != null)
        {
            transform.position = seeingEyeSigil.position;
        }
    }
}
