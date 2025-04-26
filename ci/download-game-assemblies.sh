#!/bin/bash
set -e

destination="$(dirname "$(readlink -f "$0")")/../lib"
github_url="https://api.github.com/repos/${GITHUB_REPO}/releases"

#https://docs.github.com/en/rest/releases/releases#get-the-latest-release
release=$(
    curl -L -s \
      -H "Accept: application/vnd.github+json" \
      -H "Authorization: Bearer ${GITHUB_TOKEN}" \
      -H "X-GitHub-Api-Version: 2022-11-28" \
      "$github_url/latest"
)
description=$(echo "$release" | jq -r '.body')
asset_id=$(echo  | jq -r '.assets[0] .id')

# https://docs.github.com/en/rest/reference/repos#get-a-release-asset
curl -L -s \
     -H "Accept: application/octet-stream" \
     -H "Authorization: Bearer ${GITHUB_TOKEN}" \
     -H "X-GitHub-Api-Version: 2022-11-28" \
        "$github_url/assets/${asset_id}" \
     | tar --overwrite -xzf - -C "$destination"

echo "$description"
echo "sha256"
pushd "$destination" > /dev/null
    find . -type f -name "*.dll" -printf '%P\n' | xargs shasum -a 256 -b | sort
popd > /dev/null