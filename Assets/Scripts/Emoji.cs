using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emoji : MonoBehaviour
{
    [SerializeField] private HostUDP host;

    private int id;

    public void SendEmoji()
    {
        host.emojiID = id;
    }
}
