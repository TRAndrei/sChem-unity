using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElementScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private TargetJoint2D mouseSpring;
    private Rigidbody2D rigidBody;
    private UnityEngine.UI.Text textBox;

    void Awake()
    {
        mouseSpring = gameObject.GetComponent<TargetJoint2D>();
        mouseSpring.enabled = false;

        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        textBox = gameObject.GetComponentInChildren<UnityEngine.UI.Text>();

        textBox.text = "E0";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mouseSpring.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mouseSpring.enabled)
        {
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(eventData.position);

            mouseSpring.target = cursorPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mouseSpring.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ElementScript other = collision.gameObject.GetComponent<ElementScript>();

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.SendMessage("ApplyDamage", 10);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody.velocity = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
    }

    // Update is called once per frame
    void Update()
    {

    }

}
