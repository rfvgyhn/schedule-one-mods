#!/bin/bash

for cmd in perl-rename rename; do
    if command -v "$cmd" >/dev/null; then
        RENAME_CMD="$cmd"
        break
    fi
done

if [ -z "$RENAME_CMD" ]; then
    echo "Error: Neither 'rename' nor 'perl-rename' was found" >&2
    exit 1
fi

target=win-x64
version=$(grep -oPm1 "(?<=<VersionPrefix>)[^<]+" src/Directory.Build.props)
release_name="schedule-one-mods_v${version}"
artifacts_path=artifacts
release_path="$artifacts_path/$release_name"

dotnet restore -r $target
dotnet publish src/ScheduleOneMods.DealsSummary/ScheduleOneMods.DealsSummary.csproj -r $target --no-restore -o "$release_path" -c Release
dotnet publish src/ScheduleOneMods.CounterPriceButton/ScheduleOneMods.CounterPriceButton.csproj -r $target --no-restore -o "$release_path" -c Release
find "$release_path"/ ! -name 'ScheduleOneMods.*.dll' -delete
$RENAME_CMD 's/ScheduleOneMods/Rfvgyhn/' "$release_path"/*

cp README.md CHANGELOG.md "$release_path"

pushd $artifacts_path
rm -f "$release_name.zip" && zip -r "$release_name.zip" "$release_name"
popd

rm -r "$release_path"
