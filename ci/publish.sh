#!/bin/bash

target="$1"
release_name="$2"
config="$3"
version_suffix="$4"
release_path="artifacts/$release_name"

dotnet publish src/ScheduleOneMods.ContractAggregates/ScheduleOneMods.ContractAggregates.csproj -r "$target" --no-restore -o "$release_path" -c "$config" -p:VersionSuffix="$version_suffix"