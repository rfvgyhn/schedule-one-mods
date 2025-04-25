#!/bin/bash

set -e

target=win-x64
version=$(grep -oPm1 "(?<=<VersionPrefix>)[^<]+" src/Directory.Build.props)
release_name="schedule-one-mods_v${version}"
artifacts_path=artifacts
release_path="$artifacts_path/$release_name"

dotnet restore -r $target
dotnet publish src/ScheduleOneMods.DealsSummary/ScheduleOneMods.DealsSummary.csproj -r $target --no-restore -o "$release_path" -c Release -p:AssemblyName="Rfvgyhn.DealsSummary"
dotnet publish src/ScheduleOneMods.CounterPriceButton/ScheduleOneMods.CounterPriceButton.csproj -r $target --no-restore -o "$release_path" -c Release -p:AssemblyName="Rfvgyhn.CounterPriceButton"
find "$release_path"/ ! -name 'Rfvgyhn.*.dll' -type f -delete

cp README.md CHANGELOG.md "$release_path"

pushd $artifacts_path
rm -f "$release_name.zip" && zip -r "$release_name.zip" "$release_name"
popd

rm -r "$release_path"
