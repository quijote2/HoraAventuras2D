using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;
    public int vida = 3;

    public float fuerzaSalto = 10f; 
    public float fuerzaRebote = 6f; 
    public float longitudRaycast = 0.1f; 
    public LayerMask capaSuelo; 

    private bool enSuelo; 
    private bool recibiendoDanio;
    private bool atacando;
    public bool muerto;

    private Rigidbody2D rb; 

    public Animator animator;
    public GameObject rangoEspada; // Referencia al objeto RangoEspada

    private Vector3 rangoEspadaOffsetDerecha = new Vector3(0.7f, 0f, 0f);
    private Vector3 rangoEspadaOffsetIzquierda = new Vector3(-0.7f, 0f, 0f);
    private int direccion = 1; // 1 = derecha, -1 = izquierda

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rangoEspada != null)
            rangoEspada.SetActive(false); // Asegura que la espada esté desactivada al inicio
    }

    // Update is called once per frame
    void Update()
    {
        if (!muerto)
        {
            if (!atacando)
            {
                Movimiento();

                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
                enSuelo = hit.collider != null;

                if (enSuelo && Input.GetKeyDown(KeyCode.Space) && !recibiendoDanio)
                {
                    rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                }
            }

            if (Input.GetKeyDown(KeyCode.Z) && !atacando && enSuelo)
            {
                Atacando();
            }
        }
        
        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("recibeDanio", recibiendoDanio);
        animator.SetBool("Atacando", atacando);
        animator.SetBool("muerto", muerto);
    }
       
    public void Movimiento()
    {
        float velocidadX = Input.GetAxis("Horizontal") * Time.deltaTime * velocidad;

        animator.SetFloat("movement", velocidadX * velocidad);

        if (velocidadX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            direccion = -1;
        }
        if (velocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            direccion = 1;
        }

        // Cambia la posición del rango de la espada según la dirección
        if (rangoEspada != null)
        {
            if (direccion == 1)
                rangoEspada.transform.localPosition = rangoEspadaOffsetDerecha;
            else
                rangoEspada.transform.localPosition = rangoEspadaOffsetIzquierda;
        }

        Vector3 posicion = transform.position;

        if (!recibiendoDanio)
            transform.position = new Vector3(velocidadX + posicion.x, posicion.y, posicion.z);
    }
    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if(!recibiendoDanio)
        {
            recibiendoDanio = true;
            vida -= cantDanio;
            if (vida<=0)
            {
                muerto = true;
            }
            if (!muerto)
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            }
        }
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void Atacando()
    {
        atacando = true;
        animator.SetBool("Atacando", true); // Activa la animación de ataque
        Debug.Log("¡Atacando!");
    }

    public void DesactivaAtaque()
    {
        atacando = false;
        animator.SetBool("Atacando", false); // Desactiva la animación de ataque
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}
