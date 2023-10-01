/*
 * File: SerializationVersion.cs
 * Project: DressingFramework
 * Created Date: Saturday, Aug 23th 2023, 09:12:11 am
 * Author: chocopoi (poi@chocopoi.com)
 * -----
 * Copyright (c) 2023 chocopoi
 * 
 * This file is part of DressingFramework.
 * 
 * DressingFramework is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using Newtonsoft.Json;

namespace Chocopoi.DressingFramework.Serialization
{
    /// <summary>
    /// Serialization version converter
    /// </summary>
    public class SerializationVersionConverter : JsonConverter<SerializationVersion>
    {
        public override SerializationVersion ReadJson(JsonReader reader, Type objectType, SerializationVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new SerializationVersion((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, SerializationVersion value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    /// <summary>
    /// Serialization version. Just a thin implementation of Major.Minor.Patch. Not really comply with SemVer.
    /// </summary>
    [JsonConverter(typeof(SerializationVersionConverter))]
    public class SerializationVersion
    {
        /// <summary>
        /// Major
        /// </summary>
        public int Major { get; private set; }

        /// <summary>
        /// Minor
        /// </summary>
        public int Minor { get; private set; }

        /// <summary>
        /// Patch
        /// </summary>
        public int Patch { get; private set; }

        /// <summary>
        /// Extra. Anything that is behind the hyphen.
        /// </summary>
        public string Extra { get; private set; }

        /// <summary>
        /// Creates a new serialization version
        /// </summary>
        /// <param name="major">Major</param>
        /// <param name="minor">Minor</param>
        /// <param name="patch">Patch</param>
        public SerializationVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Extra = null;
        }

        /// <summary>
        /// Creates a new serialization version
        /// </summary>
        /// <param name="major">Major</param>
        /// <param name="minor">Minor</param>
        /// <param name="patch">Patch</param>
        /// <param name="extra">Extra</param>
        public SerializationVersion(int major, int minor, int patch, string extra)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Extra = extra;
        }

        /// <summary>
        /// Creates a new serialization version from string directly
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="ArgumentException"></exception>
        public SerializationVersion(string str)
        {
            var hyphenIndex = str.IndexOf('-');
            string versionStr = null;
            if (hyphenIndex != -1)
            {
                Extra = str.Substring(hyphenIndex + 1);
                versionStr = str.Substring(0, hyphenIndex);
            }
            else
            {
                versionStr = str;
            }

            if (versionStr.Length == 0)
            {
                throw new ArgumentException("Version part is empty");
            }

            var splits = versionStr.Split('.');
            if (splits.Length < 2)
            {
                throw new ArgumentException("Version string is not in major, minor or optionally patch format");
            }

            if (!int.TryParse(splits[0], out var major))
            {
                throw new ArgumentException("Could not parse major: " + splits[0]);
            }
            Major = major;

            if (!int.TryParse(splits[1], out var minor))
            {
                throw new ArgumentException("Could not parse minor: " + splits[1]);
            }
            Minor = minor;

            if (splits.Length > 2)
            {
                if (!int.TryParse(splits[2], out var patch))
                {
                    throw new ArgumentException("Could not parse patch: " + splits[2]);
                }
                Patch = patch;
            }
            else
            {
                Patch = 0;
            }
        }

        /// <summary>
        /// Converts into string
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            var output = string.Format("{0}.{1}.{2}", Major, Minor, Patch);
            if (Extra != null)
            {
                output += "-" + Extra;
            }
            return output;
        }
    }
}
