﻿using System.Collections.Generic;
using UnityEngine;

public static class SimplePoolHelper
{
	// Pool container for given type
	private static class PoolsOfType<T> where T : class
	{
		// Pool with poolName = null
		private static SimplePool<T> defaultPool = null;

		// Other pools
		private static Dictionary<string, SimplePool<T>> namedPools = null;

		public static SimplePool<T> GetPool( string poolName = null )
		{
			if( poolName == null )
			{
				if( defaultPool == null )
					defaultPool = new SimplePool<T>();

				return defaultPool;
			}
			else
			{
				SimplePool<T> result;

				if( namedPools == null )
				{
					namedPools = new Dictionary<string, SimplePool<T>>();

					result = new SimplePool<T>();
					namedPools.Add( poolName, result );
				}
				else if( !namedPools.TryGetValue( poolName, out result ) )
				{
					result = new SimplePool<T>();
					namedPools.Add( poolName, result );
				}

				return result;
			}
		} 
	}

	// NOTE: if you don't need two or more pools of same type,
	// leave poolName as null while calling any of these functions
	// for better performance

	public static SimplePool<T> GetPool<T>( string poolName = null ) where T : class
	{
		return PoolsOfType<T>.GetPool( poolName );
	}

	public static void Push<T>( T obj, string poolName = null ) where T : class
	{
		PoolsOfType<T>.GetPool( poolName ).Push( obj );
	}

	public static T Pop<T>( string poolName = null ) where T : class
	{
		return PoolsOfType<T>.GetPool( poolName ).Pop();
	}

	// Extension method as a shorthand for Push function
	public static void Pool<T>( this T obj, string poolName = null ) where T : class
	{
		PoolsOfType<T>.GetPool( poolName ).Push( obj );
	}

    // Simple pool initializer that set the go inactive when pushed and active when popped.
    public static SimplePool<T> PopulateSimplePool<T>(SimplePool<T> simplePool, GameObject blueprint, string name, int size, Transform poolParent) where T : Component {
        simplePool = GetPool<T>(name);
        simplePool.OnPush = (item) => item.gameObject.SetActive(false);
        simplePool.OnPop = (item) => item.gameObject.SetActive(true);

        simplePool.CreateFunction = (template) =>
        {
            T newObject = GameObject.Instantiate(blueprint).GetComponent<T>();
            if (newObject) {
                newObject.transform.parent = poolParent;
            }

            return newObject;
        };
        simplePool.Populate(size);
        return simplePool;
    }
}
