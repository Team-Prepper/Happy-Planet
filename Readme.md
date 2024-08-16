## 개요
- 행성을 돌리며 시간의 흐름을 조절할 수 있는 플레이어가 행성을 키우고 꾸미는 게임

## 개발도구
- Unity 2022.3.42f1
  - https://docs.unity3d.com/Manual/PostProcessingOverview.html
- Firebase
  - 올바르게 빌드되기 위해서는 다음의 Firebase 패키지를 설치해야 합니다.
    - Firestore
    - Firebase Authentication
- NewtonJson
  - Json Parshing에 사용되었습니다.
- EHTool
  - UI, 데이터베이스, 언어 시스템에 사용되었습니다.

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
- [ ] 언어 변경

## 라이선스
- 이 프로젝트는 MIT 라이센스에 따라 배포됩니다.
- 자세한 내용은 LICENSE 파일을 참고해주세요.

## 연락
- skysea001010@gmail.com

## 참고 자료
- https://roystan.net/articles/outline-shader/
- https://gofogo.tistory.com/64