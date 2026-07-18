using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private List<Transform> noteSpawnPositions;
    [SerializeField] private Transform noteMissLine;


    public NoteObject SpawnNewNote(int index)
    {
        NoteObject noteObject = Instantiate(notePrefab, noteSpawnPositions[index].position, notePrefab.transform.rotation).GetComponent<NoteObject>();
        noteObject.SetXThereshold(noteMissLine.transform.position.x);
        return noteObject;
    }
}
