﻿namespace kitchencli.api
{
    public enum DispatchCourierMatchEnum
    {
        Unknown =0,
        M = 1,
        F = 2
    }

    interface IKitchen
    {
        void Start();

        void Initialize(string jsonFile, DispatchCourierMatchEnum dipatcherType);

        void Stop();
    }
}
