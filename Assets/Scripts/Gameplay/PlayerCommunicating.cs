using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCommunicating : MonoBehaviour
{
    [SerializeField] private InputActionReference sendMessage;
    [SerializeField] private GameObject exclamationMarkReference;
    [SerializeField] private float showTime;
    [SerializeField] private float cooldown;

    private GameObject exclamationMark;
    private bool showing = false;

    private void Start()
    {
        sendMessage.action.Enable();
        sendMessage.action.performed += SendMessageAction;
    }
    private void SendMessageAction(InputAction.CallbackContext context)
    {
        if (!showing)
        {
            showing = true;
            StartCoroutine(ShowExclamationMark());
        }
    }

    private IEnumerator ShowExclamationMark()
    {
        exclamationMark = Instantiate(exclamationMarkReference, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 50, this.gameObject.transform.position.z), Quaternion.identity, this.gameObject.transform);

        yield return new WaitForSeconds(showTime);

        Destroy(exclamationMark);

        yield return new WaitForSeconds(cooldown);

        showing = false;
    }

    private void OnDisable()
    {
        sendMessage.action.Disable();
        sendMessage.action.performed -= SendMessageAction;
    }
}
