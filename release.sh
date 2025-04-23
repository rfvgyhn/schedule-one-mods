#!/bin/bash

rm -f artifacts/release-notes.md
./publish.sh      
ci/checksums.sh
ci/release-notes.sh
mv -f release-notes.md artifacts/