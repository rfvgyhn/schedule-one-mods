#!/bin/bash

set -e

rm -f artifacts/release-notes.md
./publish.sh      
ci/checksums.sh
ci/release-notes.sh
mv -f release-notes.md artifacts/

if [[ $1 == "upload" ]]; then
  command -v gh >/dev/null 2>&1 || {
    echo >&2 "Error: Release not uploaded to Github. Github CLI not found"
    exit 1
  }
  version=$(grep -oPm1 "(?<=<VersionPrefix>)[^<]+" src/Directory.Build.props)
  gh release create "v$version" artifacts/*.zip artifacts/shasum.txt --draft --fail-on-no-commits --notes-file artifacts/release-notes.md
fi 