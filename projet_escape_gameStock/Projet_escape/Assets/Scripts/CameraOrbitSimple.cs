using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbitSimple : MonoBehaviour
{
        // Cube central (C-C) , plac� en enfant du joueur � sa position, invisible si possible
    [SerializeField] Transform InnerCube;

        // Distance entre la cam�ra et le point de focus
    public float CameraDist;

        // Vecteurs utilis�s pour contenir des variables temporairement
    Vector3 Rotation;
    Vector3 Position;

        // Sensitivit� de la cam�ra aux mouvements de la souris
    [Range(1, 10)] public int sensitivity = 2;

    
    void Start()
    {

            // On place la cam�ra derri�re le C-C (selon son orientation)
            //note : l'orientation du C-C au d�but du programme est identique � celle du joueur
        transform.position = InnerCube.position - InnerCube.forward * CameraDist;

            // On initialise les vecteurs avec les valeurs de position et rotation (en Euler) de la cam�ra
        Rotation = transform.rotation.eulerAngles;
        Position = transform.position;
    }

    
    void Update()
    {

            // Si on presse le Clic Droit :
        if (Input.GetMouseButton(1))
        {
                // On mets � jour les composantes du Vecteur Rotation avec
                //les valeurs obtenues par le mouvement de la souris
            Rotation.x -= Input.GetAxis("Mouse Y") * sensitivity;
            Rotation.y += Input.GetAxis("Mouse X") * sensitivity;

                // Si la rotation sur l'axe X est hors des bornes souhait�es, 
                //on tronque les valeurs
            Rotation.x = Mathf.Clamp(Rotation.x, -80f, 40f);
                // Ceci �vite le d�passement de certaines positions pour la cam�ra

        }
            // Si le clic droit n'est pas maintenu, le vecteur Rotation reste inchang�


            // La rotation du cube est fix�e � celle du vecteur Rotation
        InnerCube.rotation = Quaternion.Euler(Rotation.x, Rotation.y, 0);
        
        
        
            // Le vecteur Position est mis � jour avec la position actuelle du C-C
            //avec un offset vers derri�re lui (selon sa nouvelle rotation)
        Position = InnerCube.position + Vector3.up + InnerCube.forward * CameraDist;
            // On emp�che la cam�ra de passer sous le plan du sol ou trop haut
        Position.y = Mathf.Clamp(Position.y, 0.2f, InnerCube.position.y + 10);

            // On mets � jour la position de la cam�ra gr�ce au vecteur Position
        transform.position = Position;
            // On demande � la cam�ra de regarder vers un point fix� au dessus du cube
            //pour que le joueur reste centr� sur l'�cran
        transform.LookAt(InnerCube.position + Vector3.up);

    }
}