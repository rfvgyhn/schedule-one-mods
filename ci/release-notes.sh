#!/bin/bash
set -ex

root=$(dirname "$(readlink -f "$0")")/..
source "$root/ci/projects.sh"
projects=("${@:-${default_projects[@]}}")

for project in "${projects[@]}"; do
    if [[ ${#projects[@]} -gt 1 ]]; then
        echo "# $project"
        echo
    fi
    "$root"/ci/latest-changes.sh $project
done

function gh_attest_str() {
    for project in "${projects[@]}"; do
        local artifact_name=$(find "$root/artifacts" -type f -name "*$project*.zip" -printf "%f\n" -quit)
        echo "    \`gh attestation verify $artifact_name -R rfvgyhn/schedule-one-mods\`"
    done
}
cat << EOF
-----
Verify the release artifacts are built from source by Github by either:
  1. Using the [Github CLI] to [verify] the integrity and provenance using its associated cryptographically [signed attestations]
$(gh_attest_str)
     
  2. Comparing the _shasum.txt_ contents with the _Create Checksums_ section of the job log of the [automated release] ([archive])
  
     See [wiki] for instructions on how to check the checksums of the release artifacts.

[automated release]: ${CHECKSUM_URL:-https://github.com/rfvgyhn/schedule-one-mods/actions}
[wiki]: https://github.com/rfvgyhn/schedule-one-mods/wiki/Verify-Checksums-for-a-Release
[Github CLI]: https://cli.github.com/
[verify]: https://docs.github.com/en/actions/security-for-github-actions/using-artifact-attestations/using-artifact-attestations-to-establish-provenance-for-builds
[signed attestations]: ${ATTESTATION_URL:-https://github.com/rfvgyhn/schedule-one-mods/attestations}
[archive]: $ARCHIVED_JOB_URL
EOF