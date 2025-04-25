#!/bin/bash

set -e

target="$1"
release_name="$2"
config="$3"
version_suffix="$4"
release_path="artifacts/$release_name"

dotnet publish src/ScheduleOneMods.DealsSummary/ScheduleOneMods.DealsSummary.csproj -r "$target" --no-restore -o "$release_path" -c "$config" -p:VersionSuffix="$version_suffix" -p:AssemblyName="Rfvgyhn.DealsSummary"
dotnet publish src/ScheduleOneMods.CounterPriceButton/ScheduleOneMods.CounterPriceButton.csproj -r "$target" --no-restore -o "$release_path" -c "$config" -p:VersionSuffix="$version_suffix" -p:AssemblyName="Rfvgyhn.CounterPriceButton"
find "$release_path"/ ! -name 'Rfvgyhn.*.dll' -type f -delete