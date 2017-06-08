# ObjectPool

ObjectPool is an object pooling utiltiy for Unity designed to require as few changes as possible to begin using.

To spawn a pooled object of something such as a projecteile prefab, one can simply call `ObjectPool.Spawn` instead of `Instantiate` and `ObjectPool.Despawn` instead of `Destroy`. No components need to be added by the user, no special prefabs need to be added to the scene, and the arguments previously used by Instantiate and Destroy do not need to be changed for their ObjectPool equivalents.

More advanced control and monitoring is available to the user, if desired. Please see [the Wiki](https://github.com/cablay/ObjectPool/wiki/ObjectPool) for more information.
