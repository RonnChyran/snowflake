﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Snowflake.Input.Controller.Mapped;
using Snowflake.Input.Device;

namespace Snowflake.Input.Controller.Mapped
{
    /// <summary>
    /// Provides a store for <see cref="IControllerElementMappings"/>.
    /// </summary>
    public interface IControllerElementMappingsStore
    {
        /// <summary>
        /// Add the given mappings under the given profile name. Profile names do not have to be unique
        /// across mappings between different spec controllers and real devices, but must be unique
        /// for the same controller ID and device ID.
        /// </summary>
        /// <param name="mappings">The <see cref="IControllerElementMappings"/> to store.</param>
        /// <param name="profileName">The profile name to store the mappings under.</param>
        void AddMappings(IControllerElementMappings mappings, string profileName);

        /// <summary>
        /// Asynchronously add the given mappings under the given profile name. Profile names do not have to be unique
        /// across mappings between different spec controllers and real devices, but must be unique
        /// for the same controller ID and device ID.
        /// </summary>
        /// <param name="mappings">The <see cref="IControllerElementMappings"/> to store.</param>
        /// <param name="profileName">The profile name to store the mappings under.</param>
        Task AddMappingsAsync(IControllerElementMappings mappings, string profileName);

        /// <summary>
        /// Deletes all mappings from the provided controller ID to device.
        /// </summary>
        /// <param name="controllerId">The Stone controller ID that maps to the real device.</param>
        /// <param name="deviceName">The name of the device to delete mappings.</param>
        /// <param name="vendorId">The vendor ID of the device.</param>
        void DeleteMappings(ControllerId controllerId, string deviceName, int vendorId);

        /// <summary>
        /// Asynchronously deletes all mappings from the provided controller ID to device.
        /// </summary>
        /// <param name="controllerId">The Stone controller ID that maps to the real device.</param>
        /// <param name="deviceName">The name of the device to delete mappings.</param>
        /// <param name="vendorId">The vendor ID of the device.</param>
        Task DeleteMappingsAsync(ControllerId controllerId, string deviceName, int vendorId);

        /// <summary>
        /// Deletes the mapping profile from the provided controller ID to device ID.
        /// </summary>
        /// <param name="controllerId">The Stone controller ID that maps to the real device.</param>
        /// <param name="driverType">The driver for which the mapping is for.</param>
        /// <param name="deviceName">The name of the device.</param>
        /// <param name="vendorId">The vendor ID of the device.</param>
        /// <param name="profileName">The name of the mapping profile.</param>
        void DeleteMappings(ControllerId controllerId, InputDriverType driverType,
            string deviceName, int vendorId, string profileName);

        /// <summary>
        /// Asynchronously deletes the mapping profile from the provided controller ID to device ID.
        /// </summary>
        /// <param name="controllerId">The Stone controller ID that maps to the real device.</param>
        /// <param name="driverType">The driver for which the mapping is for.</param>
        /// <param name="deviceName">The name of the device.</param>
        /// <param name="vendorId">The vendor ID of the device.</param>
        /// <param name="profileName">The name of the mapping profile.</param>
        Task DeleteMappingsAsync(ControllerId controllerId, InputDriverType driverType,
            string deviceName, int vendorId, string profileName);

        /// <summary>
        /// Gets all saved mappings from the provided controller ID to device ID.
        /// </summary>
        /// <param name="controllerId">The Stone controller ID that maps to the real device.</param>
        /// <param name="deviceName">The device ID that maps from the spec controller.</param>
        /// <param name="vendorId">The vendor ID of the device.</param>
        /// <returns>All saved mappings from the provided controller ID to device ID.</returns>
        IEnumerable<IControllerElementMappings> GetMappings(ControllerId controllerId, string deviceName, int vendorId);

        /// <summary>
        /// Asynchronously gets all saved mappings from the provided controller ID to device ID.
        /// </summary>
        /// <param name="controllerId">The Stone controller ID that maps to the real device.</param>
        /// <param name="deviceName">The device ID that maps from the spec controller.</param>
        /// <param name="vendorId">The vendor ID of the device.</param>
        /// <returns>All saved mappings from the provided controller ID to device ID.</returns>
        IAsyncEnumerable<IControllerElementMappings> GetMappingsAsync(ControllerId controllerId, string deviceName, int vendorId);

        /// <summary>
        /// Gets the saved mapping profile from the provided controller ID to device ID.
        /// </summary>
        /// <param name="controllerId">The Stone controller ID that maps to the real device.</param>
        /// <param name="driverType">The driver for which the mapping is for.</param>
        /// <param name="deviceId">The device ID that maps from the spec controller.</param>
        /// <param name="vendorId">The vendor ID of the device.</param>
        /// <param name="profileName">The name of the mapping profile.</param>
        /// <returns>The saved mapping profile from the provided controller ID to device ID.</returns>
        IControllerElementMappings? GetMappings(ControllerId controllerId, InputDriverType driverType, 
            string deviceId, int vendorId, string profileName);

        /// <summary>
        /// Asynchronously gets the saved mapping profile from the provided controller ID to device ID.
        /// </summary>
        /// <param name="controllerId">The Stone controller ID that maps to the real device.</param>
        /// <param name="driverType">The driver for which the mapping is for.</param>
        /// <param name="deviceId">The device ID that maps from the spec controller.</param>
        /// <param name="vendorId">The vendor ID of the device.</param>
        /// <param name="profileName">The name of the mapping profile.</param>
        /// <returns>The saved mapping profile from the provided controller ID to device ID.</returns>
        Task<IControllerElementMappings?> GetMappingsAsync(ControllerId controllerId, InputDriverType driverType,
            string deviceId, int vendorId, string profileName);

        /// <summary>
        /// Updates the specific mapping profile with the given profile name.
        /// </summary>
        /// <param name="mappings">The <see cref="IControllerElementMappings"/> to store.</param>
        /// <param name="profileName">The profile name to store the mappings under.</param>
        void UpdateMappings(IControllerElementMappings mappings, string profileName);

        /// <summary>
        /// Asynchronously updates the specific mapping profile with the given profile name.
        /// </summary>
        /// <param name="mappings">The <see cref="IControllerElementMappings"/> to store.</param>
        /// <param name="profileName">The profile name to store the mappings under.</param>
        Task UpdateMappingsAsync(IControllerElementMappings mappings, string profileName);
    }
}
