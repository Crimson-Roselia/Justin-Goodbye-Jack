using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform noteSpawnPosition;
    [SerializeField] private Transform noteMissLine;


    public void SpawnNewNote()
    {
        NoteObject noteObject = Instantiate(notePrefab, noteSpawnPosition.position, notePrefab.transform.rotation).GetComponent<NoteObject>();
        noteObject.SetXThereshold(noteMissLine.transform.position.x);
    }
}
