import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { AuthService } from 'src/app/services/auth.service';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import { UserStoreService } from 'src/app/services/user-store.service';
import { MyIdeasComponent } from './my-ideas.component';

describe('MyIdeasComponent', () => {
  let component: MyIdeasComponent;
  let fixture: ComponentFixture<MyIdeasComponent>;
  let mockResponseIdeaService: jasmine.SpyObj<ResponseIdeaService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockUserStore: jasmine.SpyObj<UserStoreService>;
  let mockToast: jasmine.SpyObj<NgToastService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const responseIdeaServiceSpy = jasmine.createSpyObj('ResponseIdeaService', ['getIdeasByUser', 'deleteIdea']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const userStoreSpy = jasmine.createSpyObj('UserStoreService', ['getEmailFromStore', 'getUserByEmail']);
    const toastSpy = jasmine.createSpyObj('NgToastService', ['success', 'error']);
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['getEmailFromToken']);

    await TestBed.configureTestingModule({
      declarations: [ MyIdeasComponent ],
      providers: [
        { provide: ResponseIdeaService, useValue: responseIdeaServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: UserStoreService, useValue: userStoreSpy },
        { provide: NgToastService, useValue: toastSpy },
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MyIdeasComponent);
    component = fixture.componentInstance;
    mockResponseIdeaService = TestBed.inject(ResponseIdeaService) as jasmine.SpyObj<ResponseIdeaService>;
    mockRouter = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    mockUserStore = TestBed.inject(UserStoreService) as jasmine.SpyObj<UserStoreService>;
    mockToast = TestBed.inject(NgToastService) as jasmine.SpyObj<NgToastService>;
    mockAuthService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar y cargar datos', () => {
    const mockEmail = 'test@example.com';
    const mockUser = { id: 1 };
    const mockIdeas = [{ id: 1, title: 'Idea 1' }, { id: 2, title: 'Idea 2' }];

    mockUserStore.getEmailFromStore.and.returnValue(of(mockEmail));
    mockAuthService.getEmailFromToken.and.returnValue(mockEmail);
    mockUserStore.getUserByEmail.and.returnValue(of(mockUser));
    mockResponseIdeaService.getIdeasByUser.and.returnValue(of(mockIdeas));

    component.ngOnInit();

    expect(mockUserStore.getEmailFromStore).toHaveBeenCalled();
    expect(mockAuthService.getEmailFromToken).toHaveBeenCalled();
    expect(mockUserStore.getUserByEmail).toHaveBeenCalledWith(mockEmail);
    expect(mockResponseIdeaService.getIdeasByUser).toHaveBeenCalledWith(mockUser.id);

    expect(component.email).toBe(mockEmail);
    expect(component.userId).toBe(mockUser.id);
    expect(component.data).toEqual(mockIdeas);
  });

  it('debería ir al detalle de la idea', () => {
    const mockIdea = { id: 1 };
    component.seeDetail(mockIdea);
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/mis-ideas', mockIdea.id]);
  });

  it('debería eliminar una idea y mostrar un mensaje de éxito', () => {
    const mockIdeaId = 1;
    const mockResponse = {};
    component.data = [{ id: mockIdeaId, title: 'Idea 1' }];

    mockResponseIdeaService.deleteIdea.and.returnValue(of(mockResponse));

    component.deleteIdea(mockIdeaId);

    expect(mockResponseIdeaService.deleteIdea).toHaveBeenCalledWith(mockIdeaId);
    expect(component.data.length).toBe(0);
    expect(mockToast.success).toHaveBeenCalled();
  });

  it('debería mostrar un mensaje de error al fallar la eliminación de una idea', () => {
    const mockIdeaId = 1;
    const mockError = { error: 'Error message' };

    mockResponseIdeaService.deleteIdea.and.returnValue(throwError(mockError));

    component.deleteIdea(mockIdeaId);

    expect(mockResponseIdeaService.deleteIdea).toHaveBeenCalledWith(mockIdeaId);
    expect(mockToast.error).toHaveBeenCalledWith({
      detail: 'Error al eliminar la idea',
      duration: 5000,
      position: 'topCenter'
    });
  });
});
