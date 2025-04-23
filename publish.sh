#!/bin/bash

target=win-x64
version=$(grep -oPm1 "(?<=<VersionPrefix>)[^<]+" src/Directory.Build.props)
release_name="schedule-one-mods_v${version}"
release_path="artifacts/$release_name"

dotnet restore -r $target
dotnet publish src/ScheduleOneMods.ContractAggregates/ScheduleOneMods.ContractAggregates.csproj -r $target --no-restore -o "$release_path" -c Release
find "$release_path"/ ! -name 'ScheduleOneMods.*.dll' -delete
rename -v ScheduleOneMods Rfvgyhn "$release_path"/*

cp README.md CHANGELOG.md "$release_path"

tar czvf "$release_path.tar.gz" -C "artifacts" "$release_name"

rm -r "$release_path"
