#!/bin/bash

set -e

project="$1"
target="$2"
release_name="$3"
config="$4"
version_suffix="$5"
release_path="artifacts/$release_name"

dotnet publish "src/ScheduleOneMods.$project/ScheduleOneMods.$project.csproj" -r "$target" --no-restore -o "$release_path" -c "$config" -p:VersionSuffix="$version_suffix" -p:AssemblyName="Rfvgyhn.$project"
find "$release_path"/ ! -name 'Rfvgyhn.*.dll' -type f -delete