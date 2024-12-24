using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Word
{
    public interface IWordMatchService
    {
        bool Match(string word);
    }
}
