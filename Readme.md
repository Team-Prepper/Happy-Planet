## 개요

- 행성을 돌리며 시간의 흐름을 조절할 수 있는 플레이어가 행성을 키우고 꾸미는 게임
- https://easy-h.itch.io/happy-planet

## 개발환경

- Unity 6000.3.11f1
  - https://docs.unity3d.com/Manual/PostProcessingOverview.html
- Firebase
  - 올바르게 빌드되기 위해서는 다음의 Firebase 패키지를 설치해야 합니다.
    - Firestore
      - Firebase Authentication
      - FirebaseDatabase
- NewtonJson - Json Parsing에 사용되었습니다.
- EasyH - UI, 데이터베이스, 언어 시스템에 사용되었습니다.

## 배포

`deploy` 브랜치에 푸시하면 GitHub Actions에서 Unity WebGL 빌드를 생성하고 itch.io에 배포합니다.

GitHub 저장소 설정의 `Secrets and variables > Actions`에 다음 값을 추가해야 합니다.

Secrets:

- `UNITY_LICENSE`: Unity 라이선스 파일(`.ulf`) 내용
- `UNITY_EMAIL`: Unity 계정 이메일
- `UNITY_PASSWORD`: Unity 계정 비밀번호
- `BUTLER_API_KEY`: itch.io Butler API 키

Variables:

- `ITCH_USER`: itch.io 사용자명
- `ITCH_GAME`: itch.io 프로젝트 slug

Unity Package Manager가 로컬 tarball을 참조하므로 `Happy Planet/GooglePackages/*.tgz` 파일도 저장소에 포함되어 있어야 합니다.

## 계획

- [x] 행성 회전에 따른 시간의 흐름
  - [x] 시간의 흐름에 따라 낮과 밤의 변화
    - [x] 시간의 흐름에 따라 유닛의 재화를 생산, 생산 취소
- [x] 행성에 새로운 유닛 추가
  - [x] 상점에서 유닛 구매
    - [x] 추가할 유닛의 위치 지정
- [x] 행성의 상태 저장
  - [x] json 형태의 로컬 저장
    - [x] Firebase로 행성 상태 업로드
- [x] 유닛 업그레이드
- [x] 로그인 시스템
  - [x] Firebase를 이용한 인증
- [x] 언어 변경
- [ ] 미션

## 라이선스

- 이 프로젝트는 MIT 라이센스에 따라 배포됩니다.
- 자세한 내용은 LICENSE 파일을 참고해주세요.

## 연락

- skysea001010@gmail.com

## 참고 자료

- [Unity Outline Shader](https://roystan.net/articles/outline-shader/)
- [Newtonsoft Json 라이브러리 추가](https://gofogo.tistory.com/64)
- [Github Actions로 유니티 빌드](https://velog.io/@bnm000215/유니티-자동화-빌드-Git-Action)
- [itch.io에 Github Actions로 배포](https://blog.erikhorton.com/2024/03/31/deploy-bevy-to-android-and-wasm.html)
