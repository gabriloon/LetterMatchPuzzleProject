# LetterMatchPuzzleProject
It is a game where you find the correct alphabet among objects scattered throughout a 2D map made with simple Unity and interact with them to guess the letter you are aiming for.

## 조작
프로젝트 내에서 조작의 경우에는 Unity Input System 그 중 InputAction 을 만들어주고 InGameManager.cs 에서 상황 및 조작 필요성에 따라 ActionMap 을 변경하는 식으로 해서 조작 시스템이 돌아가도록 구성했습니다.

게임 시작 전:
Keyboard :  Space 키
패드 : A 버튼(엑스박스 패드 기준, 다른 패드의 경우 사방향 중 남쪽에 있는 버튼) 
을 통해 게임 시작

게임 시작: 
Keyboard :  WASD
패드 :  왼쪽 스틱 , DPAD
을 통해 캐릭터 이동

Keyboard :  Space 키
패드 : A 버튼(엑스박스 패드 기준, 다른 패드의 경우 사방향 중 남쪽에 있는 버튼) 
을 통해 다른 오브젝트랑 접촉 

게임 종료 후:
Keyboard :  Space 키
패드 : A 버튼(엑스박스 패드 기준, 다른 패드의 경우 사방향 중 남쪽에 있는 버튼) 
을 통해 게임 재 시작

Keyboard :  ESC 키
패드 : B 버튼(엑스박스 패드 기준, 다른 패드의 경우 사방향 중 동쪽 에 있는 버튼) 
을 통해 게임 종료


##게임프로세스

시작 : 
InGameManager 클래스의 Awake 함수 실행
1.그걸 통해 인스턴스가 생성되고 초기화됩니다.
2.playButton이 활성화되어 시작 화면을 표시합니다.

playButton 이 입력되고 게임을 시작하려고 할 때 : 
InGameManager 클래스의 Initialize 함수가 호출되면서 
1. playButton 을 비활성화 시킵니다.
2. 활용할 리소스를 로드하고 초기화합니다.
3. 플레이어와 게임 오브젝트를 생성합니다.
4. 캐릭터를 조작 가능 상태로 설정합니다.
5. 정답판을 초기화 하면서 초기 보석을 설정된 수량으로 보석판에 배치합니다. 
그리고 보석의 ID를 랜덤하게 할당하고, 중복되지 않도록 보석판에 추가합니다.
6. 게임 내 상자 오브젝트를 생성하고 랜덤한 위치에 배치합니다.

게임 시작: 
캐릭터가 움직임 및 애니메이션: 
PlayerView 클래스에서 플레이어가 방향키를 입력했을 시에 캐릭터의 움직임을 의미하는 변수에 값을 변경 시킨 다음에 그 값에 따라 지정한 범위 내에서 이동하는 기능과 관련 애니메이션을 실행시킵니다. 
-  Move 함수
-  SetAnimation 함수

캐릭터가 상자 또는 보석에 가까이 갔을 경우: Collider2D 컴포넌트 OnTriggerEnter2D, OnTriggerExit2D 함수를 통해 어떤 오브젝트 인지 감지를 하게 합니다. 

상자 및 보석의 경우 캐릭터가 가까이 다가갔을때 이미지가 변화해야 함으로 각각 클래스 JewelView, BoxView 의 부모클래스인 InteractionUnitView 클래스의 OnTriggerEnter2D 클래스를 통해 이미지를 반영합니다. 

캐릭터가 상자 또는 보석에다가 Interaction 입력을 했을 경우: 
PlayerView 클래스의 OnInteraction 함수를 통해 접촉한 오브젝트가 상자인지, 보석인지 확인합니다. 
해당 오브젝트에 맞게 기능을 실행합니다.
상자면 BoxView 클래스의 OpenBox 함수를 실행시키고 보석이면 JewelView 클래스의 PickupJewel 함수를 실행합니다.
추가적으로 PickupJewel 함수가 실행이 끝나면 PlayerView의 SetAnimation 함수를 실행하도록 해 특별한 애니메이션을 실행합니다.
 
BoxView 클래스의 OpenBox 함수: 상자 이미지를 여는 이미지로 바꾸면서 보석을 생성합니다. 

JewelView 클래스의 PickupJewel 함수:  
보석이 보석정답판 까지 날라가는 애니메이션을 취해줍니다.
날라가고 보석정답판에 이미 있는 보석인지 아닌지를 InGameManager 의 IsCheckPlayerJewel 함수를 통해 확인합니다. 
만약 이미 있는 보석이면 화면 밖으로 날려보내는 애니메이션과 애니메이션 종류 후 JewelView 에서 활용하는 변수를 초기화 하는 작업과 오브젝트를 삭제하는 작업을 진행합니다. 
만약 없는 보석일 경우 InGameManager 의 AddJewel 함수를 통해 현재 보석 정답판에 반영하면서 InGameManager 의 CheckWinCondition 함수를 통해 현재 정답판을 다 채웠는지 아닌지 확인합니다. 

만약 정답판을 다 채웠을 경우: 
InGameManager의 EndStage 함수를 실행합니다. 
EndStage 함수에서는 캐릭터를 조작 불가능 상태로 설정하거나 스테이지를 종료하고, 필요한 초기화 작업을 수행합니다.
그리고 종료 되었다는 UI 오브젝트를 활성화

게임 종류 후: 
Space 키또는 A 버튼을 누를 경우 InGameManager의 Initialize 를 실행시키면서 설명하는 단계의 
playButton 이 입력되고 게임을 시작하려고 할 때 단계로 돌아가게 됩니다. 

##클래스 다이어그램

간단한 클래스 다이어그램을 구성하게 되면
![JewelProject 클래스 다어어그램 ](https://github.com/user-attachments/assets/328a6270-272c-43d2-9a95-d28bdd7c4af8)

InGameManager   클래스 중심인 싱글톤 패턴을 중심으로 PlayerView, JewelView, BoxView 가 구성된다고 할 수 있습니다. 

키입력 관련해서는 InputSystem 을 사용 했으며, 먼저 PlayerInput 을 구성하면서 패드랑 키보드 키를 할당하고, 그걸 하이어라키의 InGameManager 오브젝트의 PlayerInput 컴포넌트와 OnUIAButton, OnUIBButton, OnMove , OnInteraction 함수를 통해 조작에 관여했습니다.  
