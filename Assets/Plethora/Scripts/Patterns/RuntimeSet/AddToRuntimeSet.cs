namespace PlethoraV2.Patterns.RuntimeSet
{
    using UnityEngine;

    public class AddToRuntimeSet : MonoBehaviour
    {
        [SerializeReference] RuntimeSet.Group group = RuntimeSet.Group.Any;
        [SerializeReference] RuntimeSet.Channel channel = RuntimeSet.Channel.A;

        private void OnEnable()
        {

            RuntimeSet.Add(group, channel, transform);
        }
        private void OnDisable()
        {
            RuntimeSet.Remove(group, channel, transform);
        }
    }
}